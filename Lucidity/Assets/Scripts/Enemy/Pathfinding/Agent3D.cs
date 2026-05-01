using UnityEngine;

public class Agent3D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float speed = 5f;
    [SerializeField] float turnSpeed = 10f;
    private Vector3[] path;
    private int targetIndex;

    private void Update()
    {

        Vector3 playerPosition = GameManager.PlayerRef.transform.position;
        PathRequestManager.RequestPath(transform.position, playerPosition, OnPathFound);
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
