using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersData : MonoBehaviour
{
	[Header("Humans")]
	public GameObject spearman;         // 1
	public GameObject veteranSpearman;  // 2
	public GameObject pikeman;          // 3
	public GameObject swordman;         // 4
	public GameObject knight;           // 5
	public GameObject knightHalberd;    // 6
	public GameObject humArcher;        // 7
	public GameObject trainedArcher;    // 8
	public GameObject hunter;           // 9
	public GameObject humMage;          // 10
	public GameObject rogue;            // 11
	[Header("Undeads")]
	public GameObject skeleton;         // 12
	public GameObject armoredSkeleton;  // 13
	public GameObject zombie;           // 14
	public GameObject armoredZombie;    // 15
	public GameObject skelArcher;       // 16
	public GameObject skelMage;         // 17
	public GameObject batWhite;         // 18
	public GameObject batRed;           // 19
	public GameObject ghost;            // 20
	public GameObject darkFigure;       // 21
	public GameObject necromancer;      // 22
	public GameObject reaper;           // 23
	[Header("Orcs")]
	public GameObject grunt;

	private GameMain manager;
	private Server server;
	private Client client;

	public void Init(GameMain manager)
	{
		this.manager = manager;
		this.server = this.manager.server;
		this.client = this.manager.client;
	}

	public IEnumerator CreateCharacter(Hex createAt, int characterId, string ownerName, bool isHero)
	{
		Vector3 position = createAt.transform.position;
		Character character = null;
		Player gameClient = null;
		if (Utility.IsServer())
		{
			gameClient = Utility.Get_Client_byString(ownerName, server.players);
		}
		if (client != null)
		{
			gameClient = Utility.Get_Client_byString(ownerName, client.players);
		}

		switch (characterId)
		{
			case 1:
				GameObject spearmanObj = Instantiate(spearman, position, Quaternion.identity);
				character = new Spearman(spearmanObj.transform, gameClient, isHero);
				break;
			case 2:
				GameObject veteranSpearmanObj = Instantiate(veteranSpearman, position, Quaternion.identity);
				character = new VeteranSpearman(veteranSpearmanObj.transform, gameClient, isHero);
				break;
			case 3:
				GameObject pikemanObj = Instantiate(pikeman, position, Quaternion.identity);
				character = new Pikeman(pikemanObj.transform, gameClient, isHero);
				break;
			case 4:
				GameObject swordmanObj = Instantiate(swordman, position, Quaternion.identity);
				character = new Swordman(swordmanObj.transform, gameClient, isHero);
				break;
			case 5:
				GameObject knightObj = Instantiate(knight, position, Quaternion.identity);
				character = new Knight(knightObj.transform, gameClient, isHero);
				break;
			case 6:
				GameObject knightHalberdObj = Instantiate(knightHalberd, position, Quaternion.identity);
				character = new KnightHalberd(knightHalberdObj.transform, gameClient, isHero);
				break;
			case 7:
				GameObject humArcherObj = Instantiate(humArcher, position, Quaternion.identity);
				character = new HumArcher(humArcherObj.transform, gameClient, isHero);
				break;
			case 8:
				GameObject trainedArcherObj = Instantiate(trainedArcher, position, Quaternion.identity);
				character = new TrainedArcher(trainedArcherObj.transform, gameClient, isHero);
				break;
			case 9:
				GameObject hunterObj = Instantiate(hunter, position, Quaternion.identity);
				character = new Hunter(hunterObj.transform, gameClient, isHero);
				break;
			case 10:
				GameObject humMageObj = Instantiate(humMage, position, Quaternion.identity);
				character = new HumMage(humMageObj.transform, gameClient, isHero);
				break;
			case 11:
				GameObject rogueObj = Instantiate(rogue, position, Quaternion.identity);
				character = new Rogue(rogueObj.transform, gameClient, isHero);
				break;
			case 12:
				GameObject skeletonObj = Instantiate(skeleton, position, Quaternion.identity);
				character = new Skeleton(skeletonObj.transform, gameClient, isHero);
				break;
			case 13:
				GameObject armSkeletonObj = Instantiate(armoredSkeleton, position, Quaternion.identity);
				character = new ArmoredSkeleton(armSkeletonObj.transform, gameClient, isHero);
				break;
			case 14:
				GameObject zombieObj = Instantiate(zombie, position, Quaternion.identity);
				character = new Zombie(zombieObj.transform, gameClient, isHero);
				break;
			case 15:
				GameObject armoredZombieObj = Instantiate(armoredZombie, position, Quaternion.identity);
				character = new ArmoredZombie(armoredZombieObj.transform, gameClient, isHero);
				break;
			case 16:
				GameObject skeArcherObj = Instantiate(skelArcher, position, Quaternion.identity);
				character = new SkelArcher(skeArcherObj.transform, gameClient, isHero);
				break;
			case 17:
				GameObject skelMageObj = Instantiate(skelMage, position, Quaternion.identity);
				character = new SkelMage(skelMageObj.transform, gameClient, isHero);
				break;
			case 18:
				GameObject batWhiteObj = Instantiate(batWhite, position, Quaternion.identity);
				character = new BatWhite(batWhiteObj.transform, gameClient, isHero);
				break;
			case 19:
				GameObject batRedObj = Instantiate(batRed, position, Quaternion.identity);
				character = new BatRed(batRedObj.transform, gameClient, isHero);
				break;
			case 20:
				GameObject ghostObj = Instantiate(ghost, position, Quaternion.identity);
				character = new Ghost(ghostObj.transform, gameClient, isHero);
				break;
			case 21:
				GameObject darkFigureObj = Instantiate(darkFigure, position, Quaternion.identity);
				character = new DarkFigure(darkFigureObj.transform, gameClient, isHero);
				break;
				//case 22:
				//	GameObject necromancerObj = Instantiate(necromancer, position, Quaternion.identity);
				//	character = new Necromancer(necromancerObj.transform, gameClient, isHero);
				//	break;
				//case 23:
				//	GameObject reaperObj = Instantiate(reaper, position, Quaternion.identity);
				//	character = new Reaper(reaperObj.transform, gameClient, isHero);
				//	break;
		}

		if (gameClient != null) Utility.Set_OwnerColor(character.tr, gameClient);
		if (isHero) gameClient.heroCharacter = character;
		if (character.tr != null) character.tr.Find("canMove").gameObject.SetActive(false);

		createAt.character = character;
		character.hex = createAt;
		manager.allCharacters.Add(character);

		yield return null;
	}

	public Character Get_CharacterById(int characterId)
	{
		Character result = null;
		switch (characterId)
		{
			case 1:
				result = new Spearman(null, null, false);
				break;
			case 2:
				result = new VeteranSpearman(null, null, false);
				break;
			case 3:
				result = new Pikeman(null, null, false);
				break;
			case 4:
				result = new Swordman(null, null, false);
				break;
			case 5:
				result = new Knight(null, null, false);
				break;
			case 6:
				result = new KnightHalberd(null, null, false);
				break;
			case 7:
				result = new HumArcher(null, null, false);
				break;
			case 8:
				result = new TrainedArcher(null, null, false);
				break;
			case 9:
				result = new Hunter(null, null, false);
				break;
			case 10:
				result = new HumMage(null, null, false);
				break;
			case 11:
				result = new Rogue(null, null, false);
				break;
			case 12:
				result = new Skeleton(null, null, false);
				break;
			case 13:
				result = new ArmoredSkeleton(null, null, false);
				break;
			case 14:
				result = new Zombie(null, null, false);
				break;
			case 15:
				result = new ArmoredZombie(null, null, false);
				break;
			case 16:
				result = new SkelArcher(null, null, false);
				break;
			case 17:
				result = new SkelMage(null, null, false);
				break;
			case 18:
				result = new BatWhite(null, null, false);
				break;
			case 19:
				result = new BatRed(null, null, false);
				break;
			case 20:
				result = new Ghost(null, null, false);
				break;
			case 21:
				result = new DarkFigure(null, null, false);
				break;
				//case 22:
				//	result = new Necromancer(null, null, false);
				//	break;
				//case 23:
				//	result = new Reaper(null, null, false);
				//	break;
		}
		return result;
	}
}
