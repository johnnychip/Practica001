using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace Characters
{
    [RequireComponent (typeof (TileMovement))]
    public class Character: MonoBehaviour
    {
        private TileMovement movement;

		[SerializeField]
		private Map.SetPathFinding pathFinding;

		[SerializeField]
		private AudioSource sonidoAtacar;

		[SerializeField]
		private AudioSource sonidoCaminar;

		[SerializeField]
		private AudioSource sonidoDano;

		[SerializeField]
		private Animator anim;


		public Map.Tilemap mytileMap;


		public int health;

		public int attack;

		public int defense;

		[SerializeField]
		private Vector2 toPoint;

        private void Awake ()
        {
            movement = GetComponent<TileMovement> ();
			Vector2 myPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
			FillPosition (myPosition);

        }

		void Start ()
		{

		}

		public void MoveTo (Vector2 tile, Action onFinish)
        {
            //TODO: pathfinding
			if(anim!=null)
			anim.SetTrigger ("move");
			sonidoCaminar.Play();
			Vector2 myPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

			QuitPosition (myPosition);

			List<Vector2> path = new List<Vector2> ();

			path = pathFinding.PathFinding (myPosition, tile);

			if (path.Count <= 0) {
			
				onFinish ();
				return;

			}
            movement.Move (path);

			StartCoroutine (WaitForMoveCompleted (onFinish));
        }

		private IEnumerator WaitForMoveCompleted (Action onFinish)
		{
			while (movement.isMoving)
				yield return new WaitForSeconds (0.5f);
			onFinish ();
			Vector2 myPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
			FillPosition (myPosition);
		}

		public void Attack (Action onFinish)
		{

			sonidoAtacar.Play ();

			Map.Tile actualTile = mytileMap.GetTileAt( new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
		
			List<Map.Tile> targets = GetTargets (actualTile, 1);
			foreach (Map.Tile tile in targets) 
			{

				if (tile.Character != null) {
					tile.Character.DealDamage (attack);
				
				}

			}
		
			onFinish ();
		}

		public void SuperAttack (Action onFinish)
		{
			sonidoAtacar.Play ();

			Map.Tile actualTile = mytileMap.GetTileAt( new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));

			List<Map.Tile> targets = GetTargets (actualTile, 2);
			foreach (Map.Tile tile in targets) 
			{

				if (tile.Character != null) {
					tile.Character.DealDamage (attack);
				}

			}
			onFinish ();
		}

		void FillPosition (Vector2 myPosition)
		{
			mytileMap.GetTileAt (myPosition).Character = this;
		}

		void QuitPosition (Vector2 myPosition)
		{
			mytileMap.GetTileAt (myPosition).Character = null;
		}

		public void DealDamage (int attackPoint)
		{
			
			sonidoDano.Play ();
			if(anim!=null)
				anim.SetTrigger ("damage");
			int damage = attackPoint - defense;

			health -= damage;

			if (health <= 0)
				gameObject.SetActive (false);
		}

		public List<Map.Tile> GetTargets(Map.Tile node, int range)
		{
			List<Map.Tile> targets = new List<Map.Tile>();

		

			for (int x = -range; x <= range; x++)
			{
				for (int y = -range; y <= range; y++)
				{
					if (x == 0 && y == 0)
						continue;
					
					int checkX = Mathf.RoundToInt(node.transform.position.x) + x;
					int checkY = Mathf.RoundToInt(node.transform.position.y) + y;

					if (checkX >= 0 && checkX < mytileMap.tiles[0].tiles.Length && checkY >= 0 && checkY < mytileMap.tiles.Length)
					{
						targets.Add(mytileMap.GetTileAt(new Vector2 ((float)checkX, (float)checkY)));
					}
				}


			}
			if (targets.Contains (node))
				targets.Remove (node);

			return targets;
		}

    }
}