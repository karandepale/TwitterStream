import { Routes } from '@angular/router';
import { LoginSuccessComponent } from './login-success/login-success.component';
import { LoginComponent } from './login/login.component';
import { MyTweetsComponent } from './body/my-tweets/my-tweets.component';
import { StatCardComponent } from './body/stat-card/stat-card.component';
import { BodyComponent } from './body/body.component';
import { ComposeTweetComponent } from './body/compose-tweet/compose-tweet.component';
import { SearchUsersComponent } from './body/search-users/search-users.component';
import { AnalyticsComponent } from './body/analytics/analytics.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: 'login-success',
    component: LoginSuccessComponent,
    children: [
      {
        path: '',
        component: BodyComponent,
        children: [
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
          { path: 'dashboard', component: StatCardComponent },
          { path: 'tweets', component: MyTweetsComponent },
          { path: 'Compose', component: ComposeTweetComponent },
          { path: 'Search User', component: SearchUsersComponent },
          { path: 'Analytics', component: AnalyticsComponent },
        ]
      }
    ]
  }
];

