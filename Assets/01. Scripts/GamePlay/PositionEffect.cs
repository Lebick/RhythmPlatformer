using UnityEngine;

public class PositionEffect : MonoBehaviour
{
    public GameObject particlePrefab;

    public PlayerPosition player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Vector2 lastPos = default;

            for (int i = 0; i < player.positions.Count; i += 2)
            {
                if (player.positions[i] == lastPos) continue;

                Instantiate(particlePrefab, player.positions[i], Quaternion.identity, transform);
                lastPos = player.positions[i];
            }
        }
    }
}
