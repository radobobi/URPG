using UnityEngine;
using System.Collections;

public class ActiveEffect : MonoBehaviour
{
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

    protected bool _pierces;
    public bool Pierces
    {
        get
        {
            return _pierces;
        }
        set
        {
            _pierces = value;
        }
    }

    protected string _myName;
    public string Name
    {
        get
        {
            return _myName;
        }
        set
        {
            _myName = value;
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

    protected float _endDmg;
    public float EndDamage
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

    protected float _dmgRadius;
    public float DamageRadius
    {
        get
        {
            return _dmgRadius;
        }
        set
        {
            _dmgRadius = value;
        }
    }
}
