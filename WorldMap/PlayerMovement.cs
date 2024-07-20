using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public float moveDuration = 0.25f;  // Duration to move from one tile to the next
  private Rigidbody2D rb;
  private Vector2 targetPosition;
  private bool isMoving = false;
  public TileInfoManager tim;

  private KeyPressStack movementKeysPressStack = new KeyPressStack();

  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    rb.gravityScale = 0f;  // Disable gravity
    rb.freezeRotation = true;  // Freeze rotation
    targetPosition = rb.position;  // Initialize target position
  }

  void Update()
  {
    if (!isMoving)
    {
      TryStartMoving();
    }
    UpdateKeyPressStack();
  }

  void UpdateKeyPressStack()
  {
    // Handle key presses
    if (Input.GetKeyDown(KeyCode.W))
    {
      movementKeysPressStack.Push(KeyCode.W);
    }
    if (Input.GetKeyDown(KeyCode.S))
    {
      movementKeysPressStack.Push(KeyCode.S);
    }
    if (Input.GetKeyDown(KeyCode.A))
    {
      movementKeysPressStack.Push(KeyCode.A);
    }
    if (Input.GetKeyDown(KeyCode.D))
    {
      movementKeysPressStack.Push(KeyCode.D);
    }

    // Handle key releases
    if (Input.GetKeyUp(KeyCode.W))
    {
      movementKeysPressStack.Pop(KeyCode.W);
    }
    if (Input.GetKeyUp(KeyCode.S))
    {
      movementKeysPressStack.Pop(KeyCode.S);
    }
    if (Input.GetKeyUp(KeyCode.A))
    {
      movementKeysPressStack.Pop(KeyCode.A);
    }
    if (Input.GetKeyUp(KeyCode.D))
    {
      movementKeysPressStack.Pop(KeyCode.D);
    }

  }

  void TryStartMoving()
  {
    /**
     * Attempt to start moving in the specified direction
     * If the destination tile is NOT walkable, then the movement input is effectively ignored.
     * Otherwise, the dest tile is walkable so a movement coroutine is started.
     * The IsMoving flag will be set in this case until movement is finished, and signal that
     * most direction based inputs should be ignored. 
     */
    // Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    Vector2 input = GetInputDirection();

    if (input != Vector2.zero)
    {
      Vector2 direction = input.normalized;
      Vector2 nextPosition = RoundToNearestTile(rb.position + direction);

      if (tim.GetTileAtWorldPosition(nextPosition).isWalkable)
      {
        targetPosition = nextPosition;
        StartCoroutine(MoveToTarget());
      }
    }
  }

  System.Collections.IEnumerator MoveToTarget()
  {
    isMoving = true;
    Vector2 startPosition = rb.position;
    float elapsedTime = 0f;

    while (elapsedTime < moveDuration)
    {
      rb.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
      elapsedTime += Time.fixedDeltaTime;
      yield return new WaitForFixedUpdate();
    }

    rb.position = targetPosition;  // Ensure the final position is exactly the target position
    isMoving = false;
  }

  Vector2 RoundToNearestTile(Vector2 position)
  {
    return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y));
  }




  Vector2 GetInputDirection()
  {
    KeyCode? topKey = movementKeysPressStack.GetTopOfStack();

    switch (topKey)
    {
        case KeyCode.W:
            return Vector2.up;
        case KeyCode.S:
            return Vector2.down;
        case KeyCode.A:
            return Vector2.left;
        case KeyCode.D:
            return Vector2.right;
        case null:
        default:
            return Vector2.zero;
    }
  }
}
