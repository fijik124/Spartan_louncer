package es._11thmeu.a3spipe.domain;

import java.util.Date;

public class RemoteServer {
	
	private int serverRevision;
	private Date buildDate;
	public int getServerRevision() {
		return serverRevision;
	}
	public void setServerRevision(int serverRevision) {
		this.serverRevision = serverRevision;
	}
	public Date getBuildDate() {
		return buildDate;
	}
	public void setBuildDate(Date buildDate) {
		this.buildDate = buildDate;
	}
	@Override
	public String toString() {
		return serverRevision + "," + buildDate.getTime();
	}
}
