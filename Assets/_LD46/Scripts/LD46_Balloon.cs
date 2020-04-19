//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: BALLOONS!!
//
//=============================================================================

﻿using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class LD46_Balloon : MonoBehaviour
	{
		public enum LD46_BalloonColor { Red, OrangeRed, Orange, YellowOrange, Yellow, GreenYellow, Green, BlueGreen, Blue, VioletBlue, Violet, RedViolet, LightGray, DarkGray, Random };

		private Hand hand;

		public GameObject popPrefab;

		public float maxVelocity = 5f;

		public float lifetime = 15f;
		public bool burstOnLifetimeEnd = false;

		public GameObject lifetimeEndParticlePrefab;
		public SoundPlayOneshot lifetimeEndSound;

		private float destructTime = 0f;
		private float releaseTime = 99999f;

		public SoundPlayOneshot collisionSound;
		private float lastSoundTime = 0f;
		private float soundDelay = 0.2f;

		private Rigidbody balloonRigidbody;

		private bool bParticlesSpawned = false;

		private static float s_flLastDeathSound = 0f;


		//-------------------------------------------------
		void Start()
		{
			destructTime = Time.time + lifetime + Random.value;
			hand = GetComponentInParent<Hand>();
			balloonRigidbody = GetComponent<Rigidbody>();
		}


		//-------------------------------------------------
		void Update()
		{
			if ( ( destructTime != 0 ) && ( Time.time > destructTime ) )
			{
				if ( burstOnLifetimeEnd )
				{
					SpawnParticles( lifetimeEndParticlePrefab, lifetimeEndSound );
				}

				Destroy( gameObject );
			}
		}


		//-------------------------------------------------
		private void SpawnParticles( GameObject particlePrefab, SoundPlayOneshot sound )
		{
			// Don't do this twice
			if ( bParticlesSpawned )
			{
				return;
			}

			bParticlesSpawned = true;

			if ( particlePrefab != null )
			{
				GameObject particleObject = Instantiate( particlePrefab, transform.position, transform.rotation ) as GameObject;
				particleObject.GetComponent<ParticleSystem>().Play();
				Destroy( particleObject, 2f );
			}

			if ( sound != null )
			{
				float lastSoundDiff = Time.time - s_flLastDeathSound;
				if ( lastSoundDiff < 0.1f )
				{
					sound.volMax *= 0.25f;
					sound.volMin *= 0.25f;
				}
				sound.Play();
				s_flLastDeathSound = Time.time;
			}
		}


		//-------------------------------------------------
		void FixedUpdate()
		{
			// Slow-clamp velocity
			if ( balloonRigidbody.velocity.sqrMagnitude > maxVelocity )
			{
				balloonRigidbody.velocity *= 0.97f;
			}
		}


		//-------------------------------------------------
		private void ApplyDamage()
		{
			SpawnParticles( popPrefab, null );
			Destroy( gameObject );
		}


		//-------------------------------------------------
		void OnCollisionEnter( Collision collision )
		{
			if ( bParticlesSpawned )
			{
				return;
			}

			Hand collisionParentHand = null;

			BalloonHapticBump balloonColliderScript = collision.gameObject.GetComponent<BalloonHapticBump>();

			if ( balloonColliderScript != null && balloonColliderScript.physParent != null )
			{
				collisionParentHand = balloonColliderScript.physParent.GetComponentInParent<Hand>();
			}

			if ( Time.time > ( lastSoundTime + soundDelay ) )
			{
				if ( collisionParentHand != null ) // If the collision was with a controller
				{
					if ( Time.time > ( releaseTime + soundDelay ) ) // Only play sound if it's not immediately after release
					{
						collisionSound.Play();
						lastSoundTime = Time.time;
					}
				}
				else // Collision was not with a controller, play sound
				{
					collisionSound.Play();
					lastSoundTime = Time.time;

				}
			}

			if ( destructTime > 0 ) // Balloon is released away from the controller, don't do the haptic stuff that follows
			{
				return;
			}

			if ( balloonRigidbody.velocity.magnitude > ( maxVelocity * 10 ) )
			{
				balloonRigidbody.velocity = balloonRigidbody.velocity.normalized * maxVelocity;
			}

			if ( hand != null )
			{
				ushort collisionStrength = (ushort)Mathf.Clamp( Util.RemapNumber( collision.relativeVelocity.magnitude, 0f, 3f, 500f, 800f ), 500f, 800f );

				hand.TriggerHapticPulse( collisionStrength );
			}
		}


		//-------------------------------------------------
		public void SetColor( LD46_BalloonColor color )
		{
			GetComponentInChildren<MeshRenderer>().material.color = LD46_BalloonColorToRGB( color );
		}


		//-------------------------------------------------
		private Color LD46_BalloonColorToRGB( LD46_BalloonColor balloonColorVar )
		{
			Color defaultColor = new Color( 255, 0, 0 );

			switch ( balloonColorVar )
			{
				case LD46_BalloonColor.Red:
					return new Color( 237, 29, 37, 255 ) / 255;
				case LD46_BalloonColor.OrangeRed:
					return new Color( 241, 91, 35, 255 ) / 255;
				case LD46_BalloonColor.Orange:
					return new Color( 245, 140, 31, 255 ) / 255;
				case LD46_BalloonColor.YellowOrange:
					return new Color( 253, 185, 19, 255 ) / 255;
				case LD46_BalloonColor.Yellow:
					return new Color( 254, 243, 0, 255 ) / 255;
				case LD46_BalloonColor.GreenYellow:
					return new Color( 172, 209, 54, 255 ) / 255;
				case LD46_BalloonColor.Green:
					return new Color( 0, 167, 79, 255 ) / 255;
				case LD46_BalloonColor.BlueGreen:
					return new Color( 108, 202, 189, 255 ) / 255;
				case LD46_BalloonColor.Blue:
					return new Color( 0, 119, 178, 255 ) / 255;
				case LD46_BalloonColor.VioletBlue:
					return new Color( 82, 80, 162, 255 ) / 255;
				case LD46_BalloonColor.Violet:
					return new Color( 102, 46, 143, 255 ) / 255;
				case LD46_BalloonColor.RedViolet:
					return new Color( 182, 36, 102, 255 ) / 255;
				case LD46_BalloonColor.LightGray:
					return new Color( 192, 192, 192, 255 ) / 255;
				case LD46_BalloonColor.DarkGray:
					return new Color( 128, 128, 128, 255 ) / 255;
				case LD46_BalloonColor.Random:
					int randomColor = Random.Range( 0, 12 );
					return LD46_BalloonColorToRGB( (LD46_BalloonColor)randomColor );
			}

			return defaultColor;
		}
	}
}
