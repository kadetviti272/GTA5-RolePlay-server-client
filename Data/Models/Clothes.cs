﻿using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Data
{
    public class Clothes
    {
        [Key]
        public int Id { get; set; }

        public int CharacterId { get; set; }
        public int MaskSlot { get; set; }
        public int MaskDraw { get; set; }
        public int TorsoSlot { get; set; }
        public int TorsoDraw { get; set; }
        public int LegsSlot { get; set; }
        public int LegsDraw { get; set; }
        public int BagsSlot { get; set; }
        public int BagsDraw { get; set; }
        public int FeetSlot { get; set; }
        public int FeetDraw { get; set; }
        public int AccessSlot { get; set; }
        public int AccessDraw { get; set; }
        public int UndershirtSlot { get; set; }
        public int UndershirtDraw { get; set; }
        public int ArmorSlot { get; set; }
        public int ArmorDraw { get; set; }
        public int TopsSlot { get; set; }
        public int TopsDraw { get; set; }
        public int HatsSlot { get; set; }
        public int HatsDraw { get; set; }
        public int GlassesSlot { get; set; }
        public int GlassesDraw { get; set; }

        public int Type { get; set; }
    }
}
