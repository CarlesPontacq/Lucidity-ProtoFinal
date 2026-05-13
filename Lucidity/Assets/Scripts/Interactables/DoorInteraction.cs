using UnityEngine;

public class DoorInteraction : ObjectInteraction
{
    [Header("Door Reference")]
    [SerializeField] private DoorController door;

    [Header("Interaction Hint")]
    public Transform interactionHintPos1;
    public Transform interactionHintPos2;

    private bool isFocused = false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!isFocused) return;

        Transform currentHint = GetCorrectHintPosition();

        InteractionFeedback.Instance.MoveInteractHint(currentHint.position);
    }

    public override void Interact()
    {
        if (door == null) return;

        door.Toggle();
    }

    public override void OnFocusEnter()
    {
        if (door == null || !door.IsInteractable) return;

        isFocused = true;
        hintPosition = GetCorrectHintPosition();

        base.OnFocusEnter();
    }

    public override void OnFocusExit()
    {
        isFocused = false;
        base.OnFocusExit();
    }

    public override void ResetState()
    {
        door.ResetToInitialState(false);
    }

    private Transform GetCorrectHintPosition()
    {
        Vector3 playerPos = GameManager.PlayerRef.transform.position;

        float d1 = Vector3.Distance(playerPos, interactionHintPos1.position);
        float d2 = Vector3.Distance(playerPos, interactionHintPos2.position);

        return d1 < d2 ? interactionHintPos1 : interactionHintPos2;
    }
}