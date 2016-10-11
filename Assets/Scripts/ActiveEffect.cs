using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum TargetType
{
    Unit = 0,
    Coordinate,
    AOE,
    LENGTH
}

public class ActiveEffect
{
    public ActiveEffect()
    {

    }

    protected Goon _source;
    protected Vector3 _startPos;
    public Goon Source
    {
        get
        {
            return _source;
        }
        set
        {
            _source = value;
            _startPos = value.MyPos;
            _myPos = value.MyPos;
        }
    }

    protected bool _collides;
    public bool Collides
    {
        get
        {
            return _collides;
        }
        set
        {
            _collides = value;
        }
    }

    protected float _pierceCoeff;
    public float PierceCoeff
    {
        get
        {
            return _pierceCoeff;
        }
        set
        {
            _pierceCoeff = value;
        }
    }

    protected Vector3 _myPos;
    public Vector3 Pos
    {
        get
        {
            return _myPos;
        }
        set
        {
            _myPos = value;
        }
    }

    public Vector3 StartPos
    {
        get
        {
            return _startPos;
        }
        set
        {
            _startPos = value;
        }
    }

    protected Skill _skillType;
    public Skill SkillType
    {
        get
        {
            return _skillType;
        }
        set
        {
            _skillType = value;
        }
    }

    protected float _ms;
    public float MoveSpeed
    {
        get
        {
            return _ms;
        }
        set
        {
            _ms = value;
        }
    }

    protected float _size;
    public float Size
    {
        get
        {
            return _size;
        }
        set
        {
            _size = value;
        }
    }

    protected float _startDmg;
    public float StartDamage
    {
        get
        {
            return _startDmg;
        }
        set
        {
            _startDmg = value;
        }
    }

    // smallest AOE's dmg to largest AOE's dmg
    protected float[] _endDmg;
    public float[] EndDamage
    {
        get
        {
            return _endDmg;
        }
        set
        {
            _endDmg = value;
        }
    }
    
    // smallest AOE to largest AOE
    protected float[] _dmgAOE;
    public float[] DamageAOE
    {
        get
        {
            return _dmgAOE;
        }
        set
        {
            _dmgAOE = value;
        }
    }

    protected int _targetType;
    public int TargetType
    {
        get
        {
            return _targetType;
        }
        set
        {
            _targetType = value;
        }
    }

    protected Goon _targetUnit;
    public Goon TargetUnit
    {
        get
        {
            return _targetUnit;
        }
        set
        {
            _targetUnit = value;
        }
    }

    protected Vector3 _targetPos;
    public Vector3 TargetPos
    {
        get
        {
            return _targetPos;
        }
        set
        {
            _targetPos = value;
        }
    }

    protected float _targetAOE;
    public float TargetAOE
    {
        get
        {
            return _targetAOE;
        }
        set
        {
            _targetAOE = value;
        }
    }

    protected List<Goon> _hitUnits;
    public List<Goon> HitUnits
    {
        get
        {
            return _hitUnits;
        }
        set
        {
            _hitUnits = value;
        }
    }

    public void addHitUnit(Goon unit)
    {
        _hitUnits.Add(unit);
    }

    public bool hasUnitBeenHit(Goon unit)
    {
        if (_hitUnits.Contains(unit))
        {
            return true;
        }
        return false;
    }
}
