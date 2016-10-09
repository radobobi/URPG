using UnityEngine;
using System.Collections;

public class Action {

    private Goon actingUnit;
    private Goon targetUnit;
    private Skill skillType;
    private int startTurn;
    private int endTurn;

    public Action(Goon acting, Goon target, Skill skill, int start, int end)
    {
        actingUnit = acting;
        targetUnit = target;
        skillType = skill;
        startTurn = start;
        endTurn = end;
    }

    public int getEndTurn()
    {
        return endTurn;
    }

    public Goon getActing()
    {
        return actingUnit;
    }

    public Goon getTarget()
    {
        return targetUnit;
    }

    public Skill getSkill()
    {
        return skillType;
    }
}
