using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersData : MonoBehaviour
{
    public GameObject spearman;
	public GameObject veteranSpearman;
	public GameObject swordman;
	public GameObject pikeman;
	public GameObject grunt;
	public GameObject skeleton;
	public GameObject skelMage;
	public GameObject humArcher;
	public GameObject humMage;
	public GameObject batWhite;
	public GameObject batRed;
	public GameObject skelArcher;
	public GameObject knight;
	public GameObject knightHalberd;

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
				GameObject swordmanObj = Instantiate(swordman, position, Quaternion.identity);
				character = new Swordman(swordmanObj.transform, gameClient, isHero);
				break;
			case 4:
				GameObject pikemanObj = Instantiate(pikeman, position, Quaternion.identity);
				character = new Pikeman(pikemanObj.transform, gameClient, isHero);
				break;
			case 5:
				// MonoBehaviour.Instantiate(grunt, position, Quaternion.identity);
				GameObject skeletonObj1 = Instantiate(skeleton, position, Quaternion.identity);
				character = new Skeleton(skeletonObj1.transform, gameClient, isHero);
				break;
			case 6:
				GameObject skeletonObj = Instantiate(skeleton, position, Quaternion.identity);
				character = new Skeleton(skeletonObj.transform, gameClient, isHero);
				break;
			case 7:
				GameObject skelMageObj = Instantiate(skelMage, position, Quaternion.identity);
				character = new SkelMage(skelMageObj.transform, gameClient, isHero);
				break;
			case 8:
				GameObject humArcherObj = Instantiate(humArcher, position, Quaternion.identity);
				character = new HumArcher(humArcherObj.transform, gameClient, isHero);
				break;
			case 9:
				GameObject humMageObj = Instantiate(humMage, position, Quaternion.identity);
				character = new HumMage(humMageObj.transform, gameClient, isHero);
				break;
			case 10:
				GameObject batWhiteObj = Instantiate(batWhite, position, Quaternion.identity);
				character = new BatWhite(batWhiteObj.transform, gameClient, isHero);
				break;
			case 11:
				GameObject batRedObj = Instantiate(batRed, position, Quaternion.identity);
				character = new BatRed(batRedObj.transform, gameClient, isHero);
				break;
			case 12:
				GameObject skeArcherObj = Instantiate(skelArcher, position, Quaternion.identity);
				character = new SkelArcher(skeArcherObj.transform, gameClient, isHero);
				break;
			case 13:
				GameObject knightObj = Instantiate(knight, position, Quaternion.identity);
				character = new Knight(knightObj.transform, gameClient, isHero);
				break;
			case 14:
				GameObject knightHalberdObj = Instantiate(knightHalberd, position, Quaternion.identity);
				character = new KnightHalberd(knightHalberdObj.transform, gameClient, isHero);
				break;
		}

		if(gameClient != null) Utility.Set_OwnerColor(character.tr, gameClient);
		if(isHero) gameClient.heroCharacter = character;

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
				 result = new Swordman(null, null, false);
				break;
			case 4:
				 result = new Pikeman(null, null, false);
				break;
			case 5:
				 result = new Skeleton(null, null, false);
				break;
			case 6:
				 result = new Skeleton(null, null, false);
				break;
			case 7:
				 result = new SkelMage(null, null, false);
				break;
			case 8:
				result = new HumArcher(null, null, false);
				break;
			case 9:
				result = new HumMage(null, null, false);
				break;
			case 10:
				result = new BatWhite(null, null, false);
				break;
			case 11:
				result = new BatRed(null, null, false);
				break;
			case 12:
				result = new SkelArcher(null, null, false);
				break;
			case 13:
				result = new Knight(null, null, false);
				break;
			case 14:
				result = new KnightHalberd(null, null, false);
				break;
		}
		return result;
	}
}
