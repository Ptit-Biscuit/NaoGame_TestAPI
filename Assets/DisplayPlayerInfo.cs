using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerInfo : MonoBehaviour {

	public Player player;

	public Text text;

	public Text text2;

	public int maxBoost = 10;

	public static bool playerUpdated = false;
	
	// Update is called once per frame
	void Update () {
		string vit, def, agi, str, speed, inte = "";

		if(!playerUpdated)
			this.updatePlayer(player, this.text2);

		vit = "vitality = " + player.vitality + "\n";
		def = "defense = " + player.defense + "\n";
		inte = "intelligence = " + player.intelligence + "\n";
		str = "strength = " + player.strength + "\n";
		agi = "agility = " + player.agility + "\n";
		speed = "speed = " + player.speed;

		this.text.text = "Caractéristiques\n" + vit + def + inte + str + agi + speed;
	}

	private void updatePlayer(Player player, Text text2) {
		if (!text2.text.Equals("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX")) {
			int[] boostFromUuid = new int[5];
			int i = 0;

			foreach(string s in text2.text.Split('-')) {
				boostFromUuid[i++] = int.Parse(Regex.Replace(s, "[^0-9]", string.Empty)) % this.maxBoost;
			}

			this.player.reset();
            this.player.intelligence += boostFromUuid[--i];
            this.player.strength += boostFromUuid[--i];
            this.player.agility += boostFromUuid[--i];
            this.player.speed += boostFromUuid[--i];

            playerUpdated = true;
		}
	}
}
