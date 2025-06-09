import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from './environment';

@Injectable({
  providedIn: 'root'
})
export class CentralizeServiceService {

  constructor(private http:HttpClient) { }

  TestApi() : Observable<any>{
    return this.http.get<any>(`${environment.baseUrl}/Login/Test`, {})
  }

  LoginWithX() : Observable<any>{
    return this.http.get<any>(`${environment.baseUrl}/Login/login`)
  }

  MyTweets(TweetUID: string): Observable<any> {
  return this.http.get<any>(`${environment.baseUrl}/TweetDashboard/GetAllTweets`, {
    params: {
      TweetUID: TweetUID.toString()
    }
  });
}

ComposeTweet(TweetUID: string, tweetContent: string): Observable<any> {
 // console.log("Compose Tweet Data on Centralize Service: "  + TweetUID, tweetContent)
  const payload = {
    TweetUID,
    tweetContent
  };
  return this.http.post<any>(`${environment.baseUrl}/TweetDashboard/ComposeTweet`, payload);
}

ComposeTweetWithMedia(formData: FormData): Observable<any> {
  return this.http.post<any>(
    `${environment.baseUrl}/TweetDashboard/ComposeTweetWithMedia`,
    formData
  );
}

SearchTweetProfiles(UserNames : string) : Observable<any>{
  //console.log("SearchTweetProfiles in the centralize service:" + UserNames);
  return this.http.get<any>(`${environment.baseUrl}/TweetDashboard/SearchTweeterProfile`,{
    params:{
      UserNames: UserNames.toString()
    }
  });
} 

GetAnalytics(ContentUrl : string) : Observable<any>{
  //console.log("GetAnalytics in the centralize service:" + ContentUrl);
  return this.http.get<any>(`${environment.baseUrl}/TweetDashboard/GetAnalytics`,{
    params:{
      ContentUrl: ContentUrl.toString()
    }
  });
} 


logout(twitterUID: string): Observable<any> {
  console.log("Logout called in Centralize Service with twitterUID:", twitterUID);
  return this.http.post<any>(
    `${environment.baseUrl}/Login/logout`,
    {}, 
    {
      params: {
        twitterUID: twitterUID
      }
    }
  );
}



}
