using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public interface IMove
{
    string Name { get; }
    EBattleType type { get; }
    void Execute(BattleManager battleManager, IMonster user, IMonster target);
}


public class IMove_Blab
{
    public int blab(MonsterBattleType casterTypes, MonsterBattleType targetTypes, IMove move)
    {
        int level = 10;
        int critical = NocabRNG.newRNG.generateInt(1, 2, true, true);
        int power = 100;
        int attack = 50;
        int defense = 70;
        float stab = calculate_STAB(casterTypes, move.type);
        float type1 = calculate_typeEffective(targetTypes, move.type);
        float type2 = calculate_typeEffective(targetTypes, move.type);

        int random = NocabRNG.newRNG.generateInt(217, 255, true, true) / 255;

        /**
           (2 * level * critical)                attack
          (---------------------- + 2) * power * -------
                        5                        defense
         (------------------------------------------------ + 2)  * stab * type1 * type2 * random
                                    50
         */
        float a = 2 * level * critical;
        float b = (a / 5) + 2;
        float c = b * power * (attack / defense);
        float d = c / 50;
        float result = (d + 2) * stab * type1 * type2 * random;
        return Math.Max(1, (int)result);
    }

    public float calculate_STAB(MonsterBattleType monType, EBattleType moveType)
    {
        if (moveType == EBattleType.None) { return 1; }
        bool anyMatch = (monType.type1 == moveType) || (monType.type2 == moveType);
        return anyMatch ? 1.5f : 1.0f;
    }

    public float calculate_typeEffective(MonsterBattleType targetTypes, EBattleType moveType)
    {
        if (moveType == EBattleType.None) { return 1; }
        if (targetTypes.type1 == EBattleType.None && targetTypes.type2 == EBattleType.None) { return 1; }

        float result = 0;
        return result;


    }
}

