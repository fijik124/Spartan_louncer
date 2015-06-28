package es._11thmeu.a3spipe.domain;

public class LocalRepository {
	
	private String name;
	private int localRevision;
	private String login;
	private String password;
	private String port;
	private String url;
	
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public int getLocalRevision() {
		return localRevision;
	}
	public void setLocalRevision(int localRevision) {
		this.localRevision = localRevision;
	}
	public String getLogin() {
		return login;
	}
	public void setLogin(String login) {
		this.login = login;
	}
	public String getPassword() {
		return password;
	}
	public void setPassword(String password) {
		this.password = password;
	}
	public String getPort() {
		return port;
	}
	public void setPort(String port) {
		this.port = port;
	}
	public String getUrl() {
		return url;
	}
	public void setUrl(String url) {
		this.url = url;
	}
	@Override
	public String toString() {
		return name + "," + localRevision + "," + login + "," + password + "," + port + "," + url;
	}
}
