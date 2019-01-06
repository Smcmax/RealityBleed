using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpriteExtruder : Extruder {

	[Tooltip("SpriteRenderer holding the sprite to extrude")]
	public SpriteRenderer m_spriteRenderer;

	[Tooltip("Dimensions of the extrusion, otherwise 1 pixel = 1x1 in-game")]
	public Vector2 m_size;

	private Vector2 m_pixelSize;
	private bool m_done;

	public override void Extrude() {
		if (!CanExtrude()) return;

		m_done = false;

		Sprite sprite = m_spriteRenderer.sprite;
		Texture2D tex = sprite.texture;
		Color[] pixels = tex.GetPixels();
		List<Vector2> vertices = new List<Vector2>();

		m_pixelSize = new Vector2(m_size.x / tex.width, m_size.y / tex.height);

		for(int row = 0; row < tex.height; row++)
			for(int col = 0; col < tex.width; col++) {
				if(row * tex.height + col < pixels.Length && pixels[row * tex.height + col].a > 0) {
					//vertices.Add(new Vector2(col * m_pixelSize.x, row * m_pixelSize.y));
					FindNextVertice(ref vertices, pixels, new Vector2(col, row), col, row, tex.width, tex.height, 1, 0);

					goto pixelLoopEnd;
				}
			}

		pixelLoopEnd:

		foreach(Vector2 v in vertices.Distinct()) Debug.Log("final vertice: " + v.x / m_pixelSize.x + ", " + v.y / m_pixelSize.y);

		GameObject extruded = Create3DMeshObject(vertices.Distinct().ToArray(), transform, gameObject.name + "Extrusion");
		extruded.transform.position += transform.position;
		extruded.transform.localScale = new Vector3(1, 1, 1);
	}

	// TODO: http://smcmax.com/s/2019-01-05_01-01-46.png

	// directions:
	// 0 = up
	// 1 = right
	// 2 = down
	// 3 = left
	private void FindNextVertice(ref List<Vector2> p_vertices, Color[] p_pixels, Vector2 p_start, int p_x, int p_y, int p_width, int p_height,
																							  int p_prevDirection, int p_directionToCheck) {
		if (m_done) return;

		int comingFromDirection = GetOppositeDirection(p_prevDirection);
		int nextDirection = GetNextDirection(comingFromDirection, p_directionToCheck);
		Vector2 newSquare = GetNextSquare(p_x, p_y, p_directionToCheck);
		int newX = (int) newSquare.x, newY = (int) newSquare.y;

		if(newSquare == p_start) { m_done = true; return; }

		if(newX < 0 || newX >= p_width || newY < 0 || newY >= p_height) {
			if(nextDirection != -1)
				FindNextVertice(ref p_vertices, p_pixels, p_start, p_x, p_y, p_width, p_height, p_prevDirection, nextDirection);

			return;
		}

		if(newY * p_height + newX < p_pixels.Length && p_pixels[newY * p_height + newX].a > 0) {
			Debug.Log(newX + ", " + newY);

			int nextSquareDirection = GetOppositeDirection(p_directionToCheck) == 0 ? 1 : 0;
			List<int> emptySquares = FindSurroundingEmptySquares(p_pixels, p_x, p_y, p_width, p_height);
			List<int> nextEmptySquares = FindSurroundingEmptySquares(p_pixels, newX, newY, p_width, p_height);

			//p_vertices.Add(new Vector2(newX * m_pixelSize.x, newY * m_pixelSize.y));

			if(nextEmptySquares.Count > 0) {
				List<int> necessaryVertices = new List<int>();

				foreach(int i in nextEmptySquares) // add corners, 0 = top-left, 1 = top-right...
					if(!necessaryVertices.Contains(i)) necessaryVertices.Add(i);

				List<int> verticesToAdd = new List<int>();

				foreach(int i in necessaryVertices) {
					if(i == p_directionToCheck) { // if we just moved towards the edge, add both visible corners
						Debug.Log("i " + i + " edge forward");
						if(!verticesToAdd.Contains(i)) verticesToAdd.Add(i);
						if(i == 3 && !verticesToAdd.Contains(0)) verticesToAdd.Add(0);
						if(!verticesToAdd.Contains(i + 1)) verticesToAdd.Add(i + 1);
					} else if(i - p_directionToCheck == 1 || i - p_directionToCheck == -3) { // if the edge is on the right side
						Debug.Log("i " + i + " edge on right side");
						if(verticesToAdd.Contains(i)) verticesToAdd.Remove(i);
						if(i == 3 && !verticesToAdd.Contains(0)) verticesToAdd.Add(0);
						if(!verticesToAdd.Contains(i + 1)) verticesToAdd.Add(i + 1);
					} else if(p_directionToCheck - i == 1 || p_directionToCheck - i == -3) { // if the edge is on the left side
						Debug.Log("i " + i + " edge on left side");
						if(!verticesToAdd.Contains(i)) verticesToAdd.Add(i);
						if(i == 3 && !verticesToAdd.Contains(0)) verticesToAdd.Add(0);
						if(!verticesToAdd.Contains(i + 1)) verticesToAdd.Add(i + 1);
					}
				}

				foreach(int i in verticesToAdd) {
					Debug.Log("vertice to add " + i);
					Vector2 padding = new Vector2(0, 0);

					if(i == 0) padding = new Vector2(0, -1);
					if(i == 1) padding = new Vector2(1, -1);
					if(i == 2) padding = new Vector2(1, 0);

					Debug.Log("adding with pad " + padding);

					p_vertices.Add(new Vector2((newX + padding.x) * m_pixelSize.x, (newY + padding.y) * m_pixelSize.y));
				}
			}// else p_vertices.Add(new Vector2(newX * m_pixelSize.x, newY * m_pixelSize.y));

			// make it go the same way as it did before if possible
			if(!nextEmptySquares.Contains(p_directionToCheck))
				nextSquareDirection = p_directionToCheck;

			// if we're following a border
			if(emptySquares.Count > 0 && nextEmptySquares.Count > 0) {
				List<int> blockedDirections = new List<int>();

				// check which directions are blocked
				for(int j = 0; j <= 3; j++)
					if(emptySquares.Contains(j) && nextEmptySquares.Contains(j))
						blockedDirections.Add(j);

				blockedDirections = blockedDirections.Distinct().ToList();

				// if there's a change in direction, go another way
				if(blockedDirections.Contains(p_directionToCheck))
					for(int j = 0; j <= 3; j++)
						if(!blockedDirections.Contains(j) && j != GetOppositeDirection(p_directionToCheck)) {
							nextSquareDirection = j;
							break;
						}
			} else if(nextEmptySquares.Count == 0) { // make sure that if the next square is completely surrounded, it goes the right way
				for(int i = 0; i <= 3; i++)
					if(i != GetOppositeDirection(p_directionToCheck) && emptySquares.Contains(i)) {
						nextSquareDirection = i;
						break;
					}
			}

			FindNextVertice(ref p_vertices, p_pixels, p_start, newX, newY, p_width, p_height, p_directionToCheck, nextSquareDirection);
		} else {
			if(nextDirection != -1)
				FindNextVertice(ref p_vertices, p_pixels, p_start, p_x, p_y, p_width, p_height, p_prevDirection, nextDirection);
		}
	}

	private Vector2 GetNextSquare(int p_x, int p_y, int p_direction) {
		Vector2 square = new Vector2(p_x, p_y);

		switch (p_direction) {
			case 0: square.y = p_y - 1; break;
			case 1: square.x = p_x + 1; break;
			case 2: square.y = p_y + 1; break;
			case 3: square.x = p_x - 1; break;
			default: break;
		}

		return square;
	}

	private List<int> FindSurroundingEmptySquares(Color[] p_pixels, int p_x, int p_y, int p_width, int p_height) {
		List<int> empty = new List<int>();

		for(int i = 0; i <= 3; i++)
			if(!HasPixel(p_pixels, p_x, p_y, p_width, p_height, i))
				empty.Add(i);

		return empty;
	}

	private bool HasPixel(Color[] p_pixels, int p_x, int p_y, int p_width, int p_height, int p_direction) {
		int index = p_y * p_height + p_x;

		switch(p_direction) {
			case 0: index -= p_height; break;
			case 1: index += 1; break;
			case 2: index += p_height; break;
			case 3: index -= 1; break;
			default: break;
		}

		if(index >= 0 && index < p_pixels.Length)
			return p_pixels[index].a > 0;

		return false;
	}

	private int GetOppositeDirection(int p_direction) {
		return p_direction - 2 < 0 ? p_direction + 2 : p_direction - 2;
	}

	private int GetNextDirection(int p_comingFrom, int p_direction) {
		if(p_direction + 1 != p_comingFrom) {
			if(p_direction + 1 <= 3)
				return p_direction + 1;
			else return -1;
		} else return GetNextDirection(p_comingFrom, p_direction + 1);
	}
}