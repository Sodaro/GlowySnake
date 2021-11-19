using SnakeUtilities;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeHead : BodyPart
{
    [SerializeField]
    private BodyPart bodyPartPrefab;

    private BodyPart tail;

    private Quaternion multiplyRotation = Quaternion.identity;

    private Coroutine deadRoutine;

    private float moveDelay = 1f;
    private float moveTimer = 0f;

    private bool canMove = false;


    private void OnEnable()
    {
        EventHandler.onGameStarted += Activate;
    }
    void Activate()
    {
        canMove = true;
    }

    private void OnDisable()
    {
        EventHandler.onGameStarted -= Activate;
    }

    void MoveForward()
    {
        prevPosition = transform.position;
        Vector3? targetPosition = SnakeGrid.Instance.GetPositionOnGridInDirection(transform.position, (Direction)transform.rotation.eulerAngles.y);
        if (targetPosition != null)
            transform.position = (Vector3)targetPosition;

        bool didCollide = SnakeGrid.Instance.HandleHeadCollisions(this);
        if (didCollide)
        {
            deadRoutine = StartCoroutine(Die());
            return;
        }
        if (next != null)
        {
            next.SetPosition(prevPosition);
        }
        if (tail != null)
        {
            SnakeGrid.Instance.ClearTile(tail.PreviousPosition);
        }
        else
            SnakeGrid.Instance.ClearTile(prevPosition);

        SnakeGrid.Instance.SetTileContent(transform.position, ContentType.SNAKE);
    }
    public void CreateSegment()
    {
        BodyPart part = Instantiate(bodyPartPrefab);
        if (tail == null)
        {
            part.transform.position = prevPosition;
            next = part;
            tail = part;
        }
        else
        {
            tail.next = part;
            part.transform.position = tail.PreviousPosition;
            tail = part;
        }
        if (moveDelay > 0.1f)
        {
            moveDelay -= 0.1f;
            moveDelay = Mathf.Clamp(moveDelay, 0.1f, 1f);
        }
    }

    IEnumerator Die()
    {
        particles.Play();
        meshRenderer.enabled = false;
        light.enabled = false;
        BodyPart part = next;
        while (part != null)
        {
            part.TriggerDeath();
            yield return new WaitForSeconds(0.2f);
            part = part.next;
        }
        deadRoutine = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;
        if (deadRoutine != null)
            return;
        //left arrow: rotate -90 degrees, rightarrow: rotate +90 degrees
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            multiplyRotation = Quaternion.AngleAxis(-90, Vector3.up);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            multiplyRotation = Quaternion.AngleAxis(90, Vector3.up);

        if (moveTimer < moveDelay)
        {
            moveTimer += Time.deltaTime;
        }
        else
        {
            transform.rotation *= multiplyRotation;
            multiplyRotation = Quaternion.identity;
            MoveForward();
            moveTimer = 0;
        }
    }
}
