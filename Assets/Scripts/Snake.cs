using UnityEngine;
using UnityEngine.SceneManagement;
using MyUtilities;
using System.Collections;

public class Snake : MonoBehaviour
{

    new Light light;
    [SerializeField]
    private GridGenerator grid;

    MeshRenderer meshRenderer;

    [SerializeField]
    BodyPart bodyPartPrefab;

    [SerializeField]
    ParticleSystem particles;

    BodyPart head;
    BodyPart tail;

    Vector3 prevPosition = Vector3.zero;
    Quaternion targetRotation = Quaternion.identity;

    Coroutine deadRoutine;

    float moveDelay = 1f;

    float moveTimer = 0f;
    //TODO: make snake automated movement, make it controlled by AI, add A*
    private void Awake()
    {
        light = GetComponent<Light>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void MoveForward()
    {
        prevPosition = transform.position;
        Vector3? targetPosition = grid.GetPositionOnGridInDirection(transform.position, (Direction)transform.rotation.eulerAngles.y);
        if (targetPosition != null)
            transform.position = (Vector3)targetPosition;

        bool didCollide = grid.HandleHeadCollisions(this);
        if (didCollide)
        {
            deadRoutine = StartCoroutine(Die());
            return;
        }
        if (head != null)
        {
            head.SetPosition(prevPosition);
        }
        if (tail != null)
        {
            grid.ClearTile(tail.prevPosition);
        }
        else
            grid.ClearTile(prevPosition);

        grid.SetContent(transform.position, gameObject);
    }
    public void CreateSegment()
    {
        BodyPart part = Instantiate(bodyPartPrefab);
        part.grid = grid;
        if (tail == null)
        {
            part.transform.position = prevPosition;
            head = part;
            tail = part;
        }
        else
        {
            tail.next = part;
            part.transform.position = tail.prevPosition;
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
        BodyPart part = head;
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
        if (deadRoutine != null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //targetPosition = transform.position + Vector3.left;
            targetRotation = Quaternion.AngleAxis(-90, Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //targetPosition = transform.position + Vector3.right;
            targetRotation = Quaternion.AngleAxis(90, Vector3.up);
        }

        if (moveTimer < moveDelay)
        {

            moveTimer += Time.deltaTime;
        }
        else
        {
            transform.rotation *= targetRotation;
            targetRotation = Quaternion.identity;
            MoveForward();
            moveTimer = 0;
        }
    }
}
