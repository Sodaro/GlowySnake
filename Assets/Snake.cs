using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    [SerializeField]
    private GridGenerator grid;

    [SerializeField]
    BodyPart bodyPartPrefab;

    BodyPart head;
    BodyPart tail;

    Coroutine moveRoutine;
    Vector3 prevPosition = Vector3.zero;
    Quaternion targetRotation = Quaternion.identity;

    float moveDelay = 1f;

    float moveTimer = 0f;
    //TODO: make snake automated movement, make it controlled by AI, add A*


    void MoveForward()
    {
        prevPosition = transform.position;
        Vector3? targetPosition = grid.GetPositionOnGridInDirection(transform.position, (GridGenerator.Direction)transform.rotation.eulerAngles.y);
        if (targetPosition != null)
            transform.position = (Vector3)targetPosition;
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        bool didCollide = grid.HandleHeadCollisions(this);
        if (didCollide)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    private void Start()
    {
        //moveRoutine = StartCoroutine(MoveForward(moveDelay));
    }

    // Update is called once per frame
    void Update()
    {

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
        //if (moveRoutine == null)
        //{
        //    transform.rotation *= targetRotation;
        //    targetRotation = Quaternion.identity;
        //    moveRoutine = StartCoroutine(MoveForward(moveDelay));
        //}
        //else
        //{

        //}

        //transform.rotation *= targetRotation;
        //if (targetPosition != null)
        //{
        //    if (!grid.IsPointInsideBounds((Vector3)targetPosition))
        //        return;

        //    transform.position = (Vector3)targetPosition;
            
        //}
    }
}
