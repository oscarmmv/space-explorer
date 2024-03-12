using Godot;
using System;

public partial class Planet : StaticBody3D
{
	[ExportCategory("Orbit")]
	[Export] private Planet _orbitalParent;

	[Export] private float _orbitalRadius;

	[Export] private float _orbitalAngle;

	[ExportCategory("Gravity")]
	[Export] public float surfaceRadius;

	[Export] private float _surfaceGravity;

	[ExportCategory("Rotation")]
	[Export] private float _dayLength;

	[Export] private bool _tidallyLocked;

	public float StandardGravitationalParameter { get; private set; }

	public Vector3 AccelerationByGravity { get; private set; }

	public override void _Ready()
	{
		StandardGravitationalParameter = _surfaceGravity * surfaceRadius * surfaceRadius;
	}

	public void Init()
	{
		if (_orbitalParent != null )
		{
			GlobalPosition = _orbitalParent.GlobalPosition + (Vector3.Forward * _orbitalRadius).Rotated(Vector3.Up, _orbitalAngle);
			ConstantLinearVelocity = _orbitalParent.ConstantLinearVelocity + _orbitalParent.GetOrbitalVelocity(GlobalPosition);
		}

		if (_tidallyLocked)
		{
			_dayLength = GetOrbitalPeriod();
		}

		ConstantAngularVelocity = -(_dayLength == 0 ? 0 : 2f * Mathf.Pi / _dayLength) * Vector3.Up;
	}
	public Vector3 GetAccelerationAtPosition(Vector3 globalPosition)
	{
		var d = globalPosition - this.GlobalPosition;
		return -d.Normalized() * StandardGravitationalParameter / d.LengthSquared();
	}


	public Vector3 GetOrbitalVelocity(Vector3 globalPosition)
	{
		var d = globalPosition - this.GlobalPosition;
		var speed = Mathf.Sqrt(StandardGravitationalParameter / d.Length());
		var direction = d.Normalized().Cross(Vector3.Up);
		
		return direction * speed;
	}
	public float GetOrbitalPeriod()
	{
		return 2f * Mathf.Pi * Mathf.Sqrt(Mathf.Pow(_orbitalRadius, 3) / _orbitalParent.StandardGravitationalParameter);
	}

	public Vector3 GetRelativeVelocityToSurface(Vector3 globalPosition, Vector3 linearVelocity)
	{
		var planetSpeed = ConstantLinearVelocity;
		var rotationalSpeed = ConstantAngularVelocity.Cross(globalPosition - GlobalPosition);
		var relativeSpeed = linearVelocity - (planetSpeed + rotationalSpeed);

		return relativeSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_orbitalParent != null)
		{
			AccelerationByGravity = _orbitalParent.GetAccelerationAtPosition(GlobalPosition) + _orbitalParent.AccelerationByGravity;
			var acc = AccelerationByGravity;
			ConstantLinearVelocity += acc * (float)delta;
			GlobalPosition += ConstantLinearVelocity * (float)delta;
		}

		GlobalRotation += ConstantAngularVelocity * (float)delta;
	}


	public int HowManyParents()
	{
		if (_orbitalParent == null)
		{
			return 0;
		}
		else
		{
			return _orbitalParent.HowManyParents() + 1;
		}
	}
}
