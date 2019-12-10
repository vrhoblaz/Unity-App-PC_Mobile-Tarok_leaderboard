namespace Firebase.Sample.Database
{
    using Firebase;
    using Firebase.Unity.Editor;
    using UnityEngine;

    public class FirebaseConnection : MonoBehaviour
    {
        // firebase initialization

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

        bool syncEstablished = false;   // when true, disable loading screen
        GameObject loadingGO = null;

        protected virtual void Start()
        {
            // handles loading screen on application start
            if (GameObject.Find("Img_Loading") != null)
            {
                loadingGO = GameObject.Find("Img_Loading");
            }

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                dependencyStatus = task.Result;

                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                    syncEstablished = true;
                }
                else
                {
                    Debug.LogError(
                      "Could not resolve all Firebase dependencies: " + dependencyStatus);

                    syncEstablished = true;
                    gameObject.GetComponent<MainMenuButtons>().OpenError("Connection Failed!",
                        "Error occured! " + dependencyStatus);
                }
            }); 
        }

        private void Update()
        {
            // loading screen disable on connection established
            if (loadingGO != null && syncEstablished)
            {
                loadingGO.SetActive(false);
            }
        }

        // Initialize the Firebase database:
        protected virtual void InitializeFirebase()
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            // NOTE: You'll need to replace this url with your Firebase App's database
            // path in order for the database connection to work correctly in editor.
            app.SetEditorDatabaseUrl("https://malo-za-salo-malo-zares.firebaseio.com/");
            if (app.Options.DatabaseUrl != null)
                app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
        }
    }
}