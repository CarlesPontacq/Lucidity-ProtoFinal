using UnityEngine;

public class Agent3D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float speed = 5f;
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float minDistanceToRecalculatePath = 10f;
    private Vector3[] path;
    private Vector3 lastPlayerPosition;
    private Vector3 offset = new Vector3(0f, 0.5f, 0f);
    private int targetIndex;
    Vector3 playerPosition;

    private void Start()
    {
        playerPosition = GameManager.PlayerRef.transform.position;
        playerPosition += offset;
        PathRequestManager.RequestPath(transform.position, playerPosition, OnPathFound);
        lastPlayerPosition = playerPosition;
    }

    private void Update()
    {
        playerPosition = GameManager.PlayerRef.transform.position;
        playerPosition += offset;

        if (path == null)
        {
            PathRequestManager.RequestPath(transform.position, playerPosition, OnPathFound);
            lastPlayerPosition = playerPosition;
            return;
        }

        if(Vector3.Distance(playerPosition, lastPlayerPosition) > minDistanceToRecalculatePath)
        {
            PathRequestManager.RequestPath(transform.position, playerPosition, OnPathFound);
            lastPlayerPosition = playerPosition;
        }
        
    }

    private void FixedUpdate()
    {
        Vector3 velocity = HandleMovement();
    }

    /// <summary>
    /// Mueve la unidad a lo largo de los waypoints del 'path' y devuelve la velocidad actual.
    /// </summary>
    Vector3 HandleMovement()
    {
        // Si no hay camino, no nos movemos.
        if (path == null || path.Length == 0)
        {
            return Vector3.zero;
        }

        // Obtener el waypoint actual
        Vector3 currentWaypoint = path[targetIndex];
        Vector3 oldPos = transform.position;

        // Moverse hacia el waypoint
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.fixedDeltaTime);

        // Calcular la velocidad real para la animación
        Vector3 velocity = (transform.position - oldPos) / Time.fixedDeltaTime;

        // Comprobar si hemos llegado al waypoint
        if (Vector3.Distance(transform.position, currentWaypoint) < 0.01f)
        {
            targetIndex++; // Ir al siguiente waypoint
            if (targetIndex >= path.Length)
            {
                path = null; // Hemos llegado al final del camino
            }
        }

        return velocity;
    }

    /// <summary>
    /// Callback que recibe el camino desde el PathRequestManager.
    /// </summary>
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful && newPath.Length > 0)
        {
            path = newPath;
            targetIndex = 0; // Reiniciamos el índice para empezar a seguir el nuevo camino
        }
        else
        {
            path = null; // No se encontró camino
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one * 0.5f);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
