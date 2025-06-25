using UnityEngine;

namespace LOK1game
{
    public class NetworkSideActivator : Actor
    {
        public void UpdateState(Pawn sender)
        {
            switch(networkVisibility)
            {
                case ENetworkVisibility.None:
                    gameObject.SetActive(false);
                    break;
                case ENetworkVisibility.Client:
                    if (sender.IsLocal)
                        gameObject.SetActive(true);
                    else
                        gameObject.SetActive(false);
                    break;
                case ENetworkVisibility.Others:
                    if (sender.IsLocal)
                        gameObject.SetActive(true);
                    else
                        gameObject.SetActive(false);
                    break;
                case ENetworkVisibility.Client | ENetworkVisibility.Others:
                    gameObject.SetActive(true);
                    break;

                default:
                    gameObject.SetActive(true);
                    break;
            }
        }
    }
}
