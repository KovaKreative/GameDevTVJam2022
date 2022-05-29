using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    void OnQuit() {
        print("Quit");
        Application.Quit();
    }
}
