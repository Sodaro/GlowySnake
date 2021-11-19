using SnakeUtilities;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    protected new Light light;
    protected MeshRenderer meshRenderer;
    [HideInInspector] public BodyPart next;
    [SerializeField] protected ParticleSystem particles;
    protected Vector3 prevPosition;
    public Vector3 PreviousPosition => prevPosition;

    protected virtual void Awake()
    {
        light = GetComponent<Light>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetPosition(Vector3 position)
    {
        //move the body part to a position, move the next body part to the old position
        prevPosition = transform.position;
        transform.position = position;
        if (next != null)
            next.SetPosition(prevPosition);
        SnakeGrid.Instance.SetTileContent(transform.position, ContentType.SNAKE);
    }

    public void TriggerDeath()
    {
        //activate particles and disable the lights and segment renderer
        particles.Play();
        light.enabled = false;
        meshRenderer.enabled = false;
    }
}
