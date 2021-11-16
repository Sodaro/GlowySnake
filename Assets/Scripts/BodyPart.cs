using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    new Light light;
    MeshRenderer meshRenderer;
    public GridGenerator grid;
    [HideInInspector] public BodyPart next;
    public Vector3 prevPosition;
    [SerializeField] ParticleSystem particles;

    private void Awake()
    {
        light = GetComponent<Light>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void SetPosition(Vector3 position)
    {
        prevPosition = transform.position;
        transform.position = position;
        if (next != null)
            next.SetPosition(prevPosition);
        grid.SetContent(transform.position, gameObject);
    }

    public void TriggerDeath()
    {
        particles.Play();
        light.enabled = false;
        meshRenderer.enabled = false;
    }
}
