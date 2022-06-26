// using UnityEngine;
//
// namespace MugCup_BlockBuilder.Editor
// {
//     public static class EditorEventManager
//     {
//         public static bool LeftMouseClicked = false;
//         public static bool LeftMouseDown    = false;
//
//         public static void PollEvents()
//         {
//             var _event = Event.current;
//
//             switch (_event.type)
//             {
//                 case EventType.MouseDown:
//
//                     if (_event.button == 0)
//                         LeftMouseDown = true;
//                     
//                     break;
//                 case EventType.MouseDrag:
//                     break;
//                 
//                 case EventType.MouseUp:
//                     Debug.Log("MOuse uP");
//                         LeftMouseDown = false;
//                     break;
//                 
//             }
//         }
//     }
// }
