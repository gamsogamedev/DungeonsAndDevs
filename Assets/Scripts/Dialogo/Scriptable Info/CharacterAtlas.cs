using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterAtlas : ScriptableObject
{
    [Serializable]
    public class SpriteTuple
    {
        public string spriteIdent;
        public Sprite charSprite;
    }
    
    [Serializable]
    public class CharacterInfo
    {
        public string characterName;
        public List<SpriteTuple> characterSpriteMap;

        public Sprite GetCharacter(int idx) => characterSpriteMap[idx].charSprite;
        public Sprite GetCharacter(string ident) => characterSpriteMap
            .Where(tuple => tuple.spriteIdent == ident)
            .Select(tuple => tuple.charSprite)
            .First();
    }

    public List<CharacterInfo> atlas;
}
