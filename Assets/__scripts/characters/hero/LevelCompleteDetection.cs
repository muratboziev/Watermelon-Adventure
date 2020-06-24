using UnityEngine;

public class LevelCompleteDetection : MonoBehaviour
{
    [Header("Links")]
    public GameManager game_man;

    string level_end_1 = "level_end_zone_1", level_end_2 = "level_end_zone_2";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(level_end_1))
        {
            game_man.level_complete_zone_1();
        }

        if (collision.gameObject.CompareTag(level_end_2))
        {
            game_man.block_hero_control(block_control:true, stop_move: true);
        }
    }

}