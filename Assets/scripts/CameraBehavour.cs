using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject camera;
    public GameObject character;

    void LateUpdate()
    {
        // Проверяем, существует ли объект character
        if (character != null)
        {
            CameraMove();
        }
        // Если character уничтожен, камера просто остаётся на месте
    }

    void CameraMove()
    {
        // Дополнительная проверка на всякий случай
        if (character != null && camera != null)
        {
            camera.transform.position = new Vector3(
                character.transform.position.x, 
                character.transform.position.y, 
                -10f
            );
        }
    }
}