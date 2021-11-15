using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public GridGenerator grid;
    public BodyPart next;
    public Vector3 prevPosition;
    public void SetPosition(Vector3 position)
    {
        prevPosition = transform.position;
        transform.position = position;
        if (next != null)
            next.SetPosition(prevPosition);
        grid.SetContent(transform.position, gameObject);
    }
}
