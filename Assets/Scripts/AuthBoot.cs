// using Firebase;
// using Firebase.Auth;
// using UnityEngine;

// public class AuthBoot : MonoBehaviour {
//   public static FirebaseAuth Auth;

//   async void Awake() {
//     await FirebaseApp.CheckAndFixDependenciesAsync();
//     Auth = FirebaseAuth.DefaultInstance;

//     if (Auth.CurrentUser == null) {
//       var res = await Auth.SignInAnonymouslyAsync(); // ← создаёт uid
//       Debug.Log("Anon UID: " + res.User.UserId);
//     } else {
//       Debug.Log("Signed as: " + Auth.CurrentUser.UserId);
//     }
//   }
// }