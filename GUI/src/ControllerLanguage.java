import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.ListView;

public class ControllerLanguage {
	private static final String EN = "English - UK";
	private static final String ZH = "简体中文";

	@FXML
	ListView<String> languageListView;
	
	ObservableList<String> data = FXCollections.observableArrayList(
            ZH, EN);
	
	Main main;
	
	public void setMain(Main main) {
		this.main = main;
		
		languageListView.setItems(data);
	}
	
	@FXML
	void applyButtonHandler(ActionEvent event) {
		switch (languageListView.getSelectionModel().getSelectedItem()) {
		case ZH:
			main.getLoader().setLanguage("zh");
			break;
		case EN:
			main.getLoader().setLanguage("en");
			break;
		}
		
		main.closeLanguage();
	}
	
	@FXML
	void cancelButtonHandler(ActionEvent event) {
		main.closeLanguage();
	}
}
