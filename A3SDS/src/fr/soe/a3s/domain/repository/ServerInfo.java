package fr.soe.a3s.domain.repository;

import java.io.Serializable;
import java.util.Date;
import java.util.HashSet;
import java.util.Set;




public class ServerInfo
  implements Serializable
{
  private static final long serialVersionUID = 7697232677958952953L;
  private int revision;
  private Date buildDate;
  private long numberOfFiles;
  private long totalFilesSize;
  private Set<String> hiddenFolderPaths = new HashSet<String>();
  
  private int numberOfConnections = 1;
  
  private boolean noPartialFileTransfer = false;
  
  public boolean repositoryContentUpdated = false;
  
  public boolean compressedPboFilesOnly = false;
  
  public int getRevision() {
    return this.revision;
  }
  
  public void setRevision(int revision) {
    this.revision = revision;
  }
  
  public Date getBuildDate() {
    return this.buildDate;
  }
  
  public void setBuildDate(Date buildDate) {
    this.buildDate = buildDate;
  }
  
  public long getNumberOfFiles() {
    return this.numberOfFiles;
  }
  
  public void setNumberOfFiles(long numberOfFiles) {
    this.numberOfFiles = numberOfFiles;
  }
  
  public long getTotalFilesSize() {
    return this.totalFilesSize;
  }
  
  public void setTotalFilesSize(long totalFilesSize) {
    this.totalFilesSize = totalFilesSize;
  }
  
  public Set<String> getHiddenFolderPaths() {
    if (this.hiddenFolderPaths == null) {
      this.hiddenFolderPaths = new HashSet<String>();
    }
    return this.hiddenFolderPaths;
  }
  
  public int getNumberOfConnections() {
    return this.numberOfConnections;
  }
  
  public void setNumberOfConnections(int numberOfConnections) {
    this.numberOfConnections = numberOfConnections;
  }
  
  public boolean isRepositoryContentUpdated() {
    return this.repositoryContentUpdated;
  }
  
  public void setRepositoryContentUpdated(boolean repositoryContentUpdated) {
    this.repositoryContentUpdated = repositoryContentUpdated;
  }
  
  public boolean isNoPartialFileTransfer() {
    return this.noPartialFileTransfer;
  }
  
  public void setNoPartialFileTransfer(boolean performPartialFileTransfer) {
    this.noPartialFileTransfer = performPartialFileTransfer;
  }
  
  public boolean isCompressedPboFilesOnly() {
    return this.compressedPboFilesOnly;
  }
  
  public void setCompressedPboFilesOnly(boolean compressedPboFilesOnly) {
    this.compressedPboFilesOnly = compressedPboFilesOnly;
  }
}