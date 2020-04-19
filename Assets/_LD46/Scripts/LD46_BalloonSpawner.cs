//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Spawns balloons
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class LD46_BalloonSpawner : MonoBehaviour
	{
		public float minSpawnTime = 5f;
		public float maxSpawnTime = 15f;
		private float nextSpawnTime;
		public GameObject balloonPrefab;

		public bool autoSpawn = true;
		public bool spawnAtStartup = true;

		public bool playSounds = true;
		public SoundPlayOneshot inflateSound;
		public SoundPlayOneshot stretchSound;

		public bool sendSpawnMessageToParent = false;

		public float scale = 1f;

		public Transform spawnDirectionTransform;
		public float spawnForce;

		public bool attachBalloon = false;

		public LD46_Balloon.LD46_BalloonColor color = LD46_Balloon.LD46_BalloonColor.Random;


		//-------------------------------------------------
		void Start()
		{
			if ( balloonPrefab == null )
			{
				return;
			}

			if ( autoSpawn && spawnAtStartup )
			{
				LD46_SpawnBalloon( color );
				nextSpawnTime = Random.Range( minSpawnTime, maxSpawnTime ) + Time.time;
			}
		}


		//-------------------------------------------------
		void Update()
		{
			if ( balloonPrefab == null )
			{
				return;
			}

			if ( ( Time.time > nextSpawnTime ) && autoSpawn )
			{
				LD46_SpawnBalloon( color );
				nextSpawnTime = Random.Range( minSpawnTime, maxSpawnTime ) + Time.time;
			}
		}


		//-------------------------------------------------
		public GameObject LD46_SpawnBalloon( LD46_Balloon.LD46_BalloonColor color = LD46_Balloon.LD46_BalloonColor.Red )
		{
			if ( balloonPrefab == null )
			{
				return null;
			}
			GameObject balloon = Instantiate( balloonPrefab, transform.position, transform.rotation ) as GameObject;
			balloon.transform.localScale = new Vector3( scale, scale, scale );
			if ( attachBalloon )
			{
				balloon.transform.parent = transform;
			}

			if ( sendSpawnMessageToParent )
			{
				if ( transform.parent != null )
				{
					transform.parent.SendMessage( "OnBalloonSpawned", balloon, SendMessageOptions.DontRequireReceiver );
				}
			}

			if ( playSounds )
			{
				if ( inflateSound != null )
				{
					inflateSound.Play();
				}
				if ( stretchSound != null )
				{
					stretchSound.Play();
				}
			}
			balloon.GetComponentInChildren<LD46_Balloon>().SetColor( color );
			if ( spawnDirectionTransform != null )
			{
				balloon.GetComponentInChildren<Rigidbody>().AddForce( spawnDirectionTransform.forward * spawnForce );
			}

			return balloon;
		}


		//-------------------------------------------------
		public void LD46_SpawnBalloonFromEvent( int color )
		{
			// Copy of SpawnBalloon using int because we can't pass in enums through the event system
			LD46_SpawnBalloon( (LD46_Balloon.LD46_BalloonColor)color );
		}
	}
}
