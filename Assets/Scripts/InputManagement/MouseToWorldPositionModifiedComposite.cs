using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Assets.InputManagement
{
#if UNITY_EDITOR
    [InitializeOnLoad] // Automatically register in editor.
#endif
    // Determine how GetBindingDisplayString() formats the composite by applying
    // the  DisplayStringFormat attribute.
    [DisplayStringFormat("{mouseInput}+{Rotate90}")]
    public class MouseToWorldPositionModifiedComposite : InputBindingComposite<Vector2>
    {
        // Each part binding is represented as a field of type int and annotated with
        // InputControlAttribute. Setting "layout" restricts the controls that
        // are made available for picking in the UI.
        //
        // On creation, the int value is set to an integer identifier for the binding
        // part. This identifier can read values from InputBindingCompositeContext.
        // See ReadValue() below.

        [InputControl(layout = "Vector2")]
        public int MouseInput;

        [InputControl(layout = "Button")]
        public int Rotate0;

        [InputControl(layout = "Button")]
        public int Rotate90;

        [InputControl(layout = "Button")]
        public int Rotate180;

        [InputControl(layout = "Button")]
        public int Rotate270;

        // This method computes the resulting input value of the composite based
        // on the input from its part bindings.

        CustomVector2Comparer comparer = new CustomVector2Comparer();
        public override Vector2 ReadValue(ref InputBindingCompositeContext context)
        {
            var mouse = context.ReadValue<Vector2, CustomVector2Comparer>(MouseInput, comparer);
            var r0 = context.ReadValue<float>(Rotate0);
            var r90 = context.ReadValue<float>(Rotate90);
            var r180 = context.ReadValue<float>(Rotate180);
            var r270 = context.ReadValue<float>(Rotate270);

            if (r90 != 0 && r180 != 0)
            {
                r90 = 0;
                r180 = 0;
            }
            if (r90 != 0)
            {
                if (r180 != 0 || r0 != 0)
                {
                    mouse = mouse.Rotate(45);
                }
                else
                {
                    mouse = mouse.Rotate(90);
                }
            }
            if (r270 != 0)
            {
                if (r180 != 0 || r0 != 0)
                {
                    mouse = mouse.Rotate(315);
                }
                else
                {
                    mouse = mouse.Rotate(270);
                }
            }
            if (r180 != 0)
            {
                mouse *= -1;
            }

            return mouse;
        }

        // This method computes the current actuation of the binding as a whole.
        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            var mouse = context.ReadValue<Vector2, CustomVector2Comparer>(MouseInput, comparer);
            return mouse.magnitude;
        }

        static MouseToWorldPositionModifiedComposite()
        {
            // Can give custom name or use default (type name with "Composite" clipped off).
            // Same composite can be registered multiple times with different names to introduce
            // aliases.
            //
            // NOTE: Registering from the static constructor using InitializeOnLoad and
            //       RuntimeInitializeOnLoadMethod is only one way. You can register the
            //       composite from wherever it works best for you. Note, however, that
            //       the registration has to take place before the composite is first used
            //       in a binding. Also, for the composite to show in the editor, it has
            //       to be registered from code that runs in edit mode.
            InputSystem.RegisterBindingComposite<MouseToWorldPositionModifiedComposite>();
        }

        [RuntimeInitializeOnLoadMethod]
        static void Init() { } // Trigger static constructor.
    }

    class CustomVector2Comparer : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            return 1;
        }
    }
}
