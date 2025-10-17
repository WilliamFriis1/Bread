using UnityEngine;

[CreateAssetMenu(menuName = "CharacterType", fileName = "CharacterTypeProfile")]
public class CharacterTypeProfile : ScriptableObject
{
    [System.Serializable]
    public struct Character
    {
        public enum CharacterType
        {
            NPC = 0,
            COACH = 1,
            REFEREE = 2,
        }
        public CharacterType type;
        public string id;
        public Texture2D texture;
        public bool canBeBribed;
        public bool canSpreadRumor;
    }
    public Character[] characterTypes;
}
