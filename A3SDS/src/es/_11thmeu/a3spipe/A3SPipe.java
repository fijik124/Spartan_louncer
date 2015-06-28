package es._11thmeu.a3spipe;

import java.io.File;
import java.io.FileInputStream;
import java.io.ObjectInputStream;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.util.zip.GZIPInputStream;

import javax.crypto.Cipher;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SealedObject;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;

import es._11thmeu.a3spipe.domain.LocalRepository;
import es._11thmeu.a3spipe.domain.RemoteServer;
import fr.soe.a3s.domain.repository.Repository;
import fr.soe.a3s.domain.repository.ServerInfo;

public class A3SPipe {
	
	public static void main(String[] args) {
		String op = args[0];
		String path = args[1];
		
		try {
			if (op.equals("-deserializeRepository")) {
				LocalRepository localRepository = deserializeRepository(path);
				System.out.println(localRepository);
			} else if (op.equals("-deserializeServerInfo")) {
				RemoteServer remoteRepository = deserializeServerInfo(path);
				System.out.println(remoteRepository);
			}
		} catch (Exception e) {
			System.out.println(e.getStackTrace());
		}
	}
	
	/*  
	 *  Arma3Sync variables
	 */
	
	//RepositoryService.secreteKey
	private static final byte[] secretKey = new byte[] { 0x01, 0x72, 0x43,
		0x3E, 0x1C, 0x7A, 0x55, 0, 0x01, 0x72, 0x43, 0x3E, 0x1C, 0x7A,
		0x55, 0x4F };

	/*  
	 *  Private methods
	 */

	public static LocalRepository deserializeRepository(String path) {
		
		Cipher cipher = null;
		LocalRepository localRepository = null;
		try {
			cipher = Cipher.getInstance("AES");
			SecretKey key = new SecretKeySpec(secretKey, "AES");
			cipher.init(Cipher.DECRYPT_MODE, key);
		} catch (NoSuchAlgorithmException | NoSuchPaddingException | InvalidKeyException e1) {}

		try {
			File file = new File(path);
			ObjectInputStream fRo = new ObjectInputStream(
					new GZIPInputStream(new FileInputStream(file)));
			SealedObject sealedObject = (SealedObject) fRo
					.readObject();
			Repository repository = (Repository) sealedObject
					.getObject(cipher);
			fRo.close();
			if (repository != null) {
				localRepository = new LocalRepository();
				localRepository.setName(repository.getName());
				localRepository.setLocalRevision(repository.getRevision());
				localRepository.setLogin(repository.getProtocole().getLogin());
				localRepository.setPassword(repository.getProtocole().getPassword());
				localRepository.setPort(repository.getProtocole().getPort());
				localRepository.setUrl(repository.getProtocole().getUrl());
			}
		} catch (Exception e) {}
		
		return localRepository;
	}
	
	public static RemoteServer deserializeServerInfo(String path) {
		
		RemoteServer remoteRepository = null;

		try {
			File file = new File(path);
			ObjectInputStream fRo = new ObjectInputStream(
					new GZIPInputStream(new FileInputStream(file)));
			ServerInfo serverInfo = (ServerInfo) fRo.readObject();
			fRo.close();
			if (serverInfo != null) {
				remoteRepository = new RemoteServer();
				remoteRepository.setServerRevision(serverInfo.getRevision());
				remoteRepository.setBuildDate(serverInfo.getBuildDate());
			}
			
		} catch (Exception e) {}
		
		return remoteRepository;
	}
}
