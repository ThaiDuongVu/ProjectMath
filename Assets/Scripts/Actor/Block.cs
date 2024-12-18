using TMPro;
using UnityEngine;

public class Block : Actor
{
    [Header("Block References")]
    public int initValue;
    [SerializeField] private TMP_Text valueText;

    [Header("Effects References")]
    [SerializeField] private ParticleSystem splashPrefab;
    [SerializeField] private AudioSource moveAudio;
    [SerializeField] private AudioSource mergeAudio;

    private int _value;
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            valueText.SetText(value.ToString());
        }
    }

    private Animator _animator;
    private static readonly int ShrinkAnimationTrigger = Animator.StringToHash("shrink");

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        Value = initValue;
    }

    #endregion

    public override void Explode()
    {
        CameraShaker.Instance.Shake(CameraShakeMode.Light);
        GameController.Instance.PlaySlowMotionEffect();
        Instantiate(splashPrefab, transform.position, Quaternion.identity);

        base.Explode();
    }

    public void Shrink()
    {
        _animator.SetTrigger(ShrinkAnimationTrigger);
    }

    private void Merge(Block other)
    {
        if (!other) return;

        Value += other.Value;
        Reactivate();
        Destroy(other.gameObject);

        // Play effects
        CameraShaker.Instance.Shake(CameraShakeMode.Light);
        Instantiate(splashPrefab, other.transform.position, Quaternion.identity);
        mergeAudio.Play();
    }

    public bool Merge(Operator @operator)
    {
        if (!@operator) return false;

        Value = @operator.Operate(Value);
        Reactivate();
        Destroy(@operator.gameObject);

        // Play effects
        CameraShaker.Instance.Shake(CameraShakeMode.Light);
        Instantiate(splashPrefab, @operator.transform.position, Quaternion.identity);
        mergeAudio.Play();

        return true;
    }

    // public bool Enter(PassHole passHole)
    // {
    //     if (!passHole) return false;
    //     if (Value != passHole.Value) return false;

    //     passHole.OnActivated();
    //     Destroy(gameObject);

    //     // Play effects
    //     CameraShaker.Instance.Shake(CameraShakeMode.Light);
    //     Instantiate(splashPrefab, passHole.transform.position, Quaternion.identity);

    //     return true;
    // }

    public override bool Move(Vector2 direction)
    {
        if (!CanMove(direction)) return false;

        // Raycast
        var hit = Physics2D.Raycast(transform.position, direction, 1f);
        if (hit)
        {
            // Merge & enter
            Merge(hit.transform.GetComponent<Operator>());
            Merge(hit.transform.GetComponent<Block>());

            // Interact
            var interactable = hit.transform.GetComponent<Interactable>();
            if (interactable && interactable.OnInteracted(this)) return true;
        }

        return base.Move(direction);
    }
}