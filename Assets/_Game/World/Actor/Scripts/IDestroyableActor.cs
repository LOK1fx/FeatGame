using System.Collections;
using UnityEngine;

namespace LOK1game
{
    public interface IDestroyableActor
    {
        IEnumerator OnActorDestroy();
    }
}
