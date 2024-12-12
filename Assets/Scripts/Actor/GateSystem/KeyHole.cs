using UnityEngine;
using UnityEngine.Events;

public class KeyHole : Actor
{
    [Header("Key Hole References")]
    [SerializeField] private UnityEvent activateEvents;
    [SerializeField] private AudioSource activateAudio;
    [SerializeField] private LineRenderer connectLine;
    [SerializeField] private Transform connectedTransform;

    private Animator _animator;
    private static readonly int ActivateAnimationTrigger = Animator.StringToHash("activate");

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        connectLine.positionCount = 2;
        connectLine.SetPositions(new[] { transform.position, connectedTransform.position });
    }

    #endregion

    public void OnActivated()
    {
        Reactivate();
        _animator.SetTrigger(ActivateAnimationTrigger);

        activateEvents.Invoke();
        activateAudio.Play();
    }
}
