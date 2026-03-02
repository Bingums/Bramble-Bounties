using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject rightDoor;
    public GameObject leftDoor;
    public GameObject topDoor;
    public GameObject bottomDoor;

    public void EnableRightDoor() => rightDoor.SetActive(true);
    public void EnableLeftDoor() => leftDoor.SetActive(true);
    public void EnableTopDoor() => topDoor.SetActive(true);
    public void EnableBottomDoor() => bottomDoor.SetActive(true);
}