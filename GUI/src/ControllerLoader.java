import java.io.File;
import java.io.IOException;

import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.TextField;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.web.WebView;
import javafx.stage.FileChooser;

public class ControllerLoader {
	@FXML
	TextField pathTextField;
	@FXML
	Button languageButton;
	@FXML
	Button goButton;
	@FXML
	ImageView langButtonIcon;
	@FXML
	ImageView goButtonIcon;
	@FXML
	WebView resultWebView;

	Main main;
	boolean pathEmpty;
	String lang;

	public void setMain(Main main) {
		this.main = main;
		pathEmpty = true;
		lang = "zh";

		resultWebView.getEngine().load(getClass().getResource("defaultPage.html").toString());
	}

	public void setLanguage(String lang) {
		this.lang = lang;

		switch (lang) {
		case "zh":
			langButtonIcon.setImage(new Image(getClass().getResource("ZH_ICON.png").toString()));
			break;
		case "en":
			langButtonIcon.setImage(new Image(getClass().getResource("EN_ICON.png").toString()));
			break;
		}
	}

	@FXML
	void browseButtonHandler(ActionEvent event) {
		FileChooser fileChooser = new FileChooser();
		fileChooser.setTitle("Open Resource File");
		fileChooser.getExtensionFilters().addAll(
				new FileChooser.ExtensionFilter("C Source File", "*.c"),
				new FileChooser.ExtensionFilter("All Files", "*.*"));

		File file = fileChooser.showOpenDialog(main.getStage());
		if (file != null) {
			pathTextField.setText(file.getPath());
		}

		checkPathTextField();
	}

	@FXML
	void goButtonHandler(ActionEvent event) {
		String workingDir = System.getProperty("user.dir");
		File f = new File(pathTextField.getText());
		if (f.exists()) {
		try {
			Process process = new ProcessBuilder(workingDir + "\\core\\ExplanationGenerator.exe",
					workingDir + "\\lang\\", pathTextField.getText(),
					workingDir + "\\result\\index.html", lang).start();

			process.waitFor();

			resultWebView.getEngine().load("file:///" + workingDir + "\\result\\index.html");
		} catch (IOException e) {
			e.printStackTrace();
		} catch (InterruptedException e) {
			e.printStackTrace();
		}} else {
			Main.showError("The file you selected doesn't exist!");
		}
	}

	@FXML
	void pathTextFieldOnKeyTyped() {
		checkPathTextField();
	}

	@FXML
	void langButtonHandler(ActionEvent event) {
		main.showLanguage();
	}

	void checkPathTextField() {
		pathEmpty = pathTextField.getText().equals("");
		goButton.setDisable(pathEmpty);
	}
}
