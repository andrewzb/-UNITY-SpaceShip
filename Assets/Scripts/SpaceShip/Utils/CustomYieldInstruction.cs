using System.Collections;

namespace SpaceShip.Utils
{
    namespace UnityEngine
    {
        public abstract class CustomYieldInstruction : IEnumerator
        {
            public abstract bool keepWaiting { get; }

            public object Current => (object)null;

            public bool MoveNext() => this.keepWaiting;

            public virtual void Reset()
            {
            }
        }
    }
}