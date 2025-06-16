import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SocialSchedulingComponent } from './social-scheduling.component';

describe('SocialSchedulingComponent', () => {
  let component: SocialSchedulingComponent;
  let fixture: ComponentFixture<SocialSchedulingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SocialSchedulingComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SocialSchedulingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
