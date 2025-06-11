import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TwitterTrendsComponent } from './twitter-trends.component';

describe('TwitterTrendsComponent', () => {
  let component: TwitterTrendsComponent;
  let fixture: ComponentFixture<TwitterTrendsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TwitterTrendsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TwitterTrendsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
