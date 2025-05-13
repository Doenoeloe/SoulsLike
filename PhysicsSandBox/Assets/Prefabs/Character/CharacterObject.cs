using UnityEngine;

[CreateAssetMenu(fileName = "CharacterObject", menuName = "Scriptable Objects/CharacterObject")]
public class CharacterObject : ScriptableObject
{
    [Header("Character Movement")]
    public float  moveSpeed;
    public float  jumpForce;
    public float  gravity;
    public float  gravityMultiplier;
    public float  rotateSpeed;
    
    [Header("Character Stats")]
    public float  health;
    public float  mana;
    public float  stamina;
    public float strength;
    public float dexterity;
    public float intelligence;
    
    [Header("Stats multiplier")]
    public float strengthMultiplier;
    public float dexterityMultiplier;
    public float intelligenceMultiplier;
}
