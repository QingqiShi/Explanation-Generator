import java.io.IOException;

import ExplanationGenerator.CoreExtractor;
import javafx.application.Application;
import javafx.application.Platform;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.control.Alert.AlertType;
import javafx.scene.layout.BorderPane;
import javafx.stage.Modality;
import javafx.stage.Stage;

public class Main extends Application {
	Stage primaryStage;
	Stage languageStage;
	ControllerLoader loaderController;
	ControllerLanguage languageController;
	
	public Stage getStage() { return primaryStage; }
	public ControllerLoader getLoader() { return loaderController; }
	
	public static void main(String[] args) {
		launch(args);
	}

	@Override
	public void start(Stage primaryStage) throws Exception {
		this.primaryStage = primaryStage;
		primaryStage.setResizable(false);
		primaryStage.setTitle("C-Code Explainer");

		showLoader();
		
		CoreExtractor.extract();
	}

	public void showLoader() {
		FXMLLoader loader = new FXMLLoader(getClass().getResource("UILoader.fxml"));
		BorderPane basePane;
		try {
			basePane = loader.load();

			loaderController = loader.getController();
			loaderController.setMain(this);

			Scene scene = new Scene(basePane);

			primaryStage.setScene(scene);
			primaryStage.show();
		} catch (IOException e) {
			showError("Can not load 'UIConnect.fxml'.");
		}
	}
	
	public void showLanguage() {
		FXMLLoader loader = new FXMLLoader(getClass().getResource("UILanguage.fxml"));
		BorderPane basePane;
		try {
			basePane = loader.load();

			languageController = loader.getController();
			languageController.setMain(this);

			Scene scene = new Scene(basePane);
			languageStage = new Stage();
			languageStage.initModality(Modality.WINDOW_MODAL);
			languageStage.initOwner(primaryStage.getScene().getWindow());
			languageStage.setScene(scene);
			languageStage.show();
		} catch (IOException e) {
			showError("Can not load 'UIConnect.fxml'.");
		}
	}
	
	public void closeLanguage() {
		languageStage.close();
	}
	
	public static void showError(String errorMsg) {
		Platform.runLater(new Runnable() {
			@Override
			public void run() {
				Alert alert = new Alert(AlertType.ERROR);
				alert.setTitle("Error Dialog");
				alert.setHeaderText("Error");
				alert.setContentText(errorMsg);

				alert.showAndWait();
			}
		});
	}
}
