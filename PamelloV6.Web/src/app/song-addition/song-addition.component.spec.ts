import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SongAdditionComponent } from './song-addition.component';

describe('SongAdditionComponent', () => {
  let component: SongAdditionComponent;
  let fixture: ComponentFixture<SongAdditionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SongAdditionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SongAdditionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
