using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using YARG.Data;
using YARG.Input;
using YARG.Util;

namespace YARG.UI {
	public partial class MainMenu : MonoBehaviour {
		public static bool isPostSong = false;

		public static MainMenu Instance {
			get;
			private set;
		}

		public SongInfo chosenSong = null;

		[SerializeField]
		private UIDocument editPlayersDocument;

		[SerializeField]
		private Canvas mainMenu;
		[SerializeField]
		private Canvas songSelect;
		[SerializeField]
		private Canvas difficultySelect;
		[SerializeField]
		private Canvas postSong;

		[SerializeField]
		private GameObject settingsMenu;

		private void Start() {
			Instance = this;

			// Load song folder from player prefs
			if (!string.IsNullOrEmpty(PlayerPrefs.GetString("songFolder"))) {
				SongLibrary.songFolder = new(PlayerPrefs.GetString("songFolder"));
			}

			if (!isPostSong) {
				ShowMainMenu();
			} else {
				ShowPostSong();
			}
		}

		private void OnEnable() {
			// Bind input events
			foreach (var player in PlayerManager.players) {
				player.inputStrategy.GenericNavigationEvent += OnGenericNavigation;
			}
		}

		private void OnDisable() {
			// Save player prefs
			PlayerPrefs.Save();

			// Unbind input events
			foreach (var player in PlayerManager.players) {
				player.inputStrategy.GenericNavigationEvent -= OnGenericNavigation;
			}
		}

		private void Update() {
			// Update player navigation
			foreach (var player in PlayerManager.players) {
				player.inputStrategy.UpdateNavigationMode();
			}
		}

		private void OnGenericNavigation(NavigationType navigationType, bool firstPressed) {
			if (!firstPressed) {
				return;
			}

			if (navigationType == NavigationType.PRIMARY) {
				ShowSongSelect();
			}
		}

		private void HideAll() {
			editPlayersDocument.SetVisible(false);

			mainMenu.gameObject.SetActive(false);
			songSelect.gameObject.SetActive(false);
			difficultySelect.gameObject.SetActive(false);
			postSong.gameObject.SetActive(false);
		}

		public void ShowMainMenu() {
			HideAll();

			settingsMenu.SetActive(false);
			mainMenu.gameObject.SetActive(true);
		}

		public void ShowEditPlayers() {
			HideAll();
			editPlayersDocument.SetVisible(true);
		}

		public void ShowSongSelect() {
			HideAll();
			songSelect.gameObject.SetActive(true);
		}

		public void ShowPreSong() {
			HideAll();
			difficultySelect.gameObject.SetActive(true);
		}

		public void ShowPostSong() {
			HideAll();
			postSong.gameObject.SetActive(true);
			isPostSong = false;
		}

		public void ToggleSettingsMenu() {
			settingsMenu.SetActive(!settingsMenu.activeSelf);
		}

		public void ShowCalibrationScene() {
			if (PlayerManager.players.Count > 0) {
				GameManager.Instance.LoadScene(SceneIndex.CALIBRATION);
			}
		}

		public void ShowHostServerScene() {
			GameManager.Instance.LoadScene(SceneIndex.SERVER_HOST);
		}

		public void RefreshCache() {
			if (SongLibrary.CacheFile.Exists) {
				File.Delete(SongLibrary.CacheFile.FullName);
				SongLibrary.Reset();
				ShowSongSelect();
			}
		}

		public void Quit() {
			Application.Quit();
		}
	}
}