using UnityEngine;
using UnityEngine.EventSystems;

namespace StickmanParty.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoBootstrap()
        {
            if (FindObjectOfType<GameManager>() == null)
            {
                GameObject bootObj = new GameObject("--- GAME BOOTSTRAP ---");
                bootObj.AddComponent<GameBootstrap>();
            }
        }

        private void Awake()
        {
            SetupCamera();
            SetupEventSystem();
            SetupManagers();
        }

        private void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }

            cam.orthographic = true;
            cam.orthographicSize = 6.2f;
            cam.backgroundColor = new Color(0.1f, 0.12f, 0.16f); // Sleek modern dark blue
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0, 0, -10f);
        }

        private void SetupEventSystem()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
            }
        }

        private void SetupManagers()
        {
            GameObject managers = new GameObject("--- MANAGERS ---");
            managers.AddComponent<PlayerManager>();
            managers.AddComponent<AudioManager>();
            managers.AddComponent<UIManager>();
            managers.AddComponent<TournamentManager>();
            managers.AddComponent<GameManager>();

            UIManager.Instance?.ShowMainMenu();
        }
    }
}
