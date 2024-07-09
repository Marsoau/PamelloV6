import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MultipageComponent } from './multipage.component';

describe('MultipageComponent', () => {
  let component: MultipageComponent;
  let fixture: ComponentFixture<MultipageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MultipageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MultipageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
