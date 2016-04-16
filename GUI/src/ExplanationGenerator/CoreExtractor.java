package ExplanationGenerator;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

public class CoreExtractor {
	public static void extract() {
		File f = new File("core/");
		f.mkdirs();
		f = new File("lang/");
		f.mkdirs();
		f = new File("result/highlight/styles");
		f.mkdirs();

		try {
			extractFile("ExplanationGenerator.exe", "core/");
			extractFile("ClangSharp.dll", "core/");
			extractFile("libclang.dll", "core/");

			f = new File("lang/zh.lang");
			if (!f.exists())
				extractFile("zh.lang", "lang/");

			f = new File("lang/en.lang");
			if (!f.exists())
				extractFile("en.lang", "lang/");

			f = new File("result/highlight/styles/dracula.css");
			if (!f.exists())
				extractFile("dracula.css", "result/highlight/styles/");

			f = new File("result/highlight/highlight.pack.js");
			if (!f.exists())
				extractFile("highlight.pack.js", "result/highlight/");
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	static void extractFile(String fileName, String dir) throws IOException {
		FileOutputStream output = new FileOutputStream(dir + fileName);
		InputStream input = new CoreExtractor().getClass().getResourceAsStream(fileName);
		byte[] buffer = new byte[4096];
		int bytesRead = input.read(buffer);
		while (bytesRead != -1) {
			output.write(buffer, 0, bytesRead);
			bytesRead = input.read(buffer);
		}
		output.close();
		input.close();
	}
}
