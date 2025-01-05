using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour
{
    private Controller player;

    private LineRenderer lineRenderer;

    [SerializeField] private float shootInterval = 1f;

    [SerializeField] private float distanceToPlayer = 500f;
    [SerializeField] private float damage = 10f;



    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Controller>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;

        StartCoroutine(CheckPlayer());
    }

    void Update()
    {
        if (player != null)
        {
            gameObject.transform.LookAt(player.GetComponent<Transform>(), Vector3.up);
        }
    }

    private IEnumerator CheckPlayer()
    {
        while (true)
        {
            if (player != null)
            {
                if ((transform.position - player.transform.position).magnitude < distanceToPlayer && player.GetSTandTime() < 0f)
                {
                    lineRenderer.enabled = true;
                    Shoot();
                }
            }
            yield return new WaitForSeconds(shootInterval / 5);

            lineRenderer.enabled = false;

            yield return new WaitForSeconds(shootInterval / 5 * 4);
        }
        yield return null;
    }

    public void Shoot()
    {
        
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
        RaycastHit hit;

        Vector3 endPosition = player.GetComponent<Transform>().position;
        if (Physics.Raycast(ray, out hit, distanceToPlayer)) {
            if (hit.collider != null)
            {
                endPosition = hit.point;
            }
        }

        if (hit.collider.name == "Player")
        {
            player.TakeDamage(damage);
        }

        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, endPosition + 0.2f * Vector3.up);

    }
}
