﻿using System;

namespace MMogri.Gameplay
{
    [System.Serializable]
    public class Player : Entity
    {
        public Guid Id;
        public Guid accountId;

        public Guid mapId;

        public int lvl;
        public int exp;

        public string playerState;

        public CharacterStats stats;

        public Player()
        {
            Id = Guid.NewGuid();
        }
    }
}
