using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    private bool pauseGame = true;
    public GameObject OnPanel = null;

    // Start is called before the first frame update
    void Start()
    {
        OnPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame = !pauseGame;

            if (pauseGame == true)
            {
                OnPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;     // 標準モード
                Cursor.visible = true;    // カーソル表示
            }
            else
            {
                OnPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;   // 中央にロック
                Cursor.visible = false;     // カーソル非表示
            }
        }
    }
}
